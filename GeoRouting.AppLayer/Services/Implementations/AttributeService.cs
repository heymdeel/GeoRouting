using AutoMapper;
using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Exceptions;
using GeoRouting.AppLayer.Model;
using GeoRouting.AppLayer.Model.Entities;
using LinqToDB;
using LinqToDB.Data;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attribute = GeoRouting.AppLayer.Model.Entities.Attribute;

namespace GeoRouting.AppLayer.Services
{
    public class AttributeService : IAttributeService
    {
        private IMapper mapper;

        public AttributeService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<Attribute> AddPointAttribute(int userId, PointAttributeInput attributeInput)
        {
            using (var db = new DbContext())
            {
                if (await db.Categories.FirstOrDefaultAsync(c => c.Id == attributeInput.CategoryId && c.IsPoint) == null)
                {
                    throw new BadInputException(101, "wrong category");
                }

                if (await db.Users.FirstOrDefaultAsync(u => u.Id == userId) == null)
                {
                    throw new BadInputException(102, "user with such id was not found");
                }

                // inserting attribute into database
                var attribute = mapper.Map<Attribute>(attributeInput);
                attribute.UserId = userId;
                attribute.IsPoint = true;
                int attributeId = await db.InsertWithInt32IdentityAsync(attribute);

                var pointAttribute = new PointAttribute()
                {
                    Id = attributeId,
                    Location = new Point(attributeInput.Location.Longitude, attributeInput.Location.Latitude)
                    {
                        SRID = 4326
                    },
                    Radius = attributeInput.Radius
                };

                await db.InsertAsync(pointAttribute);

                // searching for the collisions
                var collisonParameters = new
                {
                    id_attribute = new DataParameter { Value = attributeId},
                    lon = new DataParameter { Value = attributeInput.Location.Longitude},
                    lat = new DataParameter { Value = attributeInput.Location.Latitude},
                    radius = new DataParameter { Value = attributeInput.Radius}
                };

                var collisions = await db.QueryToListAsync<WaysAttributes>(@"select id as id_way, @id_attribute as id_attribute, 
                                                        St_length(ST_Intersection(ST_Buffer(St_Point(@lon, @lat)::geography, @radius), the_geom::geography)) / St_length(the_geom::geography) as coverage 
                                                        from ways where st_DWithin(the_geom, St_Point(@lon, @lat)::geography, @radius)", collisonParameters);
                db.BulkCopy(collisions);

                // calculating new weights of the edges
                var edgesParameters = new
                {
                    category = new DataParameter { Value = attributeInput.CategoryId },
                    id_attribute = new DataParameter { Value = attributeId }
                };

                await db.ExecuteAsync(@"update ways set cost = cost * (1 - coverage + coeff * coverage), 
                                                        reverse_cost = reverse_cost * (1 - coverage + coeff * coverage),
                                                        maxspeed_forward = maxspeed_forward / (1 - coverage + coeff * coverage), 
                                                        maxspeed_backward = maxspeed_backward / (1 - coverage + coeff * coverage) 
                                        from (select id_way, coverage, coeff 
                                            from (select id_way, @category as category, coverage from ways_attributes where id_attribute = @id_attribute) as t1
                                        inner join categories on category = categories.id) as t2 where ways.id = t2.id_way", edgesParameters);

                return await db.Attributes.FirstOrDefaultAsync(a => a.Id == attributeId);
            }
        }

        public async Task<Attribute> AddLongAttribute(int userId, LongAttributeInput attributeInput)
        {
            using (var db = new DbContext())
            {
                if (attributeInput.Points.Count() < 2)
                {
                    throw new BadInputException(106, "number of points must be 2 or more");
                }

                if (await db.Categories.FirstOrDefaultAsync(c => c.Id == attributeInput.CategoryId && c.IsLong) == null)
                {
                    throw new BadInputException(101, "wrong category");
                }

                if (await db.Users.FirstOrDefaultAsync(u => u.Id == userId) == null)
                {
                    throw new BadInputException(102, "user with such id was not found");
                }

                // inserting attribute into database
                var attribute = mapper.Map<Attribute>(attributeInput);
                attribute.UserId = userId;
                attribute.IsPoint = false;
                int attributeId = await db.InsertWithInt32IdentityAsync(attribute);

                List<int> closestPointsID = (await FindClosestNodes(attributeInput.Points)).ToList();
                List<int> affectedEdges = new List<int>();

                for (int i = 0; i < (closestPointsID.Count - 1); i++)
                {
                    var pointsParameters = new
                    {
                        source = new DataParameter { Value = closestPointsID[i] },
                        target = new DataParameter { Value = closestPointsID[i+1] }
                    };
                    var edges = await db.QueryToListAsync<int>(
                                      @"SELECT id
                                      FROM pgr_bdDijkstra('SELECT id, source, target, cost, reverse_cost
                                                          FROM ways_backup', @source, @target) 
                                      inner join ways_backup on edge = ways_backup.id",
                                      pointsParameters);

                    affectedEdges.AddRange(edges);
                }

                var collisions = affectedEdges
                                .Select(edgeId => 
                                new WaysAttributes
                                {
                                    AttributeId = attributeId,
                                    WayId = edgeId,
                                    Coverage = 1
                                });

                db.BulkCopy(collisions);

                var edgesParameters = new
                {
                    category = new DataParameter { Value = attributeInput.CategoryId },
                    id_attribute = new DataParameter { Value = attributeId }
                };

                await db.ExecuteAsync(@"update ways set cost = cost * coeff, 
                                                        reverse_cost = reverse_cost * coeff,
                                                        maxspeed_forward = maxspeed_forward / coeff,
                                                        maxspeed_backward = maxspeed_backward / coeff
                                        from (select id_way, coverage, coeff 
                                              from (select id_way, @category as category, coverage 
                                                    from ways_attributes where id_attribute = @id_attribute) as t1
                                        inner join categories on category = categories.id) as t2 where ways.id = t2.id_way", edgesParameters);

                return await db.Attributes.FirstOrDefaultAsync(a => a.Id == attributeId);   
            }
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            using (var db = new DbContext())
            {
                return await db.Categories.ToListAsync();
            }
        }

        public async Task<WayAttributesDTO> GetUserAttributes(int userId)
        {
            using (var db = new DbContext())
            {
                var parameters = new
                {
                    user_id = new DataParameter { Value = userId }
                };

                var pointAttributes = await db.QueryToListAsync<PointAttributeDTO>(
                                            @"select t1.id, t1.user as UserId, commentary, category, radius, st_x(location) as longitude, st_y(location) as latitude 
                                              from (select id, attributes.user, commentary, category 
                                                    from attributes where attributes.user = @user_id and is_point = true) as t1
                                              inner join point_attributes on t1.id = point_attributes.id", parameters);

                var longAttributes = await db.Attributes
                                             .Where(a => a.UserId == userId && !a.IsPoint)
                                             .Select(a => new LongAttrbiuteDTO
                                             {
                                                Id = a.Id,
                                                UserId = userId,
                                                Commentary = a.Commentary,
                                                Category = a.CategoryId
                                             })
                                             .ToListAsync();

                foreach (var attr in longAttributes)
                {
                    attr.Points = await db.WaysAttributes
                                    .LoadWith(wa => wa.Way)
                                    .Where(wa => wa.AttributeId == attr.Id)
                                    .Select(wa => new LongEdgeDTO
                                    {
                                        SourceLongitude = wa.Way.X1.Value,
                                        SourceLatitude = wa.Way.Y1.Value,
                                        TargetLongitude = wa.Way.X2.Value,
                                        TargetLatitude = wa.Way.Y2.Value,
                                        Length = wa.Way.LengthM.Value
                                    })
                                    .ToListAsync();
                }

                return new WayAttributesDTO
                {
                    PointAttributes = pointAttributes,
                    LongAttributes = longAttributes
                };
            }
        }

        public async Task RemoveAttribute(int attributeId, int userId)
        {
            using (var db = new DbContext())
            {
                var attribute = await db.Attributes
                                        .LoadWith(a => a.Category)
                                        .FirstOrDefaultAsync(a => a.Id == attributeId);

                if (attribute == null)
                {
                    throw new BadInputException(105, "attribute was not found");
                }

                if (attribute.UserId != userId)
                {
                    throw new AccessRefusedException(104, "access denied");
                }

                var parameters = new
                {
                    coefficient = new DataParameter { Value = attribute.Category.Coefficient },
                    id_attribute = new DataParameter { Value = attributeId }
                };

                await db.ExecuteAsync(@"update ways set cost = cost / (1 - coverage + coeff * coverage),
                                                        reverse_cost = reverse_cost / (1 - coverage + coeff * coverage),
				                                        maxspeed_forward = maxspeed_forward * (1 - coverage + coeff * coverage),
                                                        maxspeed_backward = maxspeed_backward * (1 - coverage + coeff * coverage)
                                        from (select id_way, coverage, @coefficient as coeff 
		                                      from (select id_way, coverage from ways_attributes where id_attribute = @id_attribute) as t1) as t2
                                        where ways.id = t2.id_way", parameters);

                await db.WaysAttributes.Where(w => w.AttributeId == attributeId).DeleteAsync();
                if (attribute.IsPoint)
                {
                    await db.PointAttributes.Where(a => a.Id == attributeId).DeleteAsync();
                }

                await db.Attributes.Where(a => a.Id == attributeId).DeleteAsync();
            }
        }

        private async Task<IEnumerable<int>> FindClosestNodes(IEnumerable<LocationDTO> points)
        {
            var closestNodes = await Task.WhenAll(points.Select(async p =>
            {
                using (var db = new DbContext())
                {
                    var queryParamters = new
                    {
                        lon = new DataParameter { Value = p.Longitude },
                        lat = new DataParameter { Value = p.Latitude }
                    };

                    return await db.ExecuteAsync<int>(@"select id from 
                                                            ((select x1 as x, y1 as y, source as id
	                                                            FROM ways
	                                                            ORDER BY the_geom <-> st_setsrid(st_makepoint(@lon, @lat),4326) limit 1)
	                                                            union
	                                                            (select x2 as x, y2 as y, target as id
	                                                            FROM ways
	                                                            ORDER BY the_geom <-> st_setsrid(st_makepoint(@lon, @lat),4326) limit 1)) as t1
                                                            order by st_setsrid(st_makepoint(x, y),4326) <-> st_setsrid(st_makepoint(@lon, @lat),4326)
                                                            limit 1", queryParamters);
                }
            }));

            return closestNodes;
        }
    }
}
