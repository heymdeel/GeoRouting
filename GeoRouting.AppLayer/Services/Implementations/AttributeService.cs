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

                // searching for collisions
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
                                        maxspeed_forward = maxspeed_forward / (1 - coverage + coeff * coverage) from 
                                        (select id_way, coverage, coeff 
                                            from (select id_way, @category as category, coverage from ways_attributes where id_attribute = @id_attribute) as t1
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
    }
}
