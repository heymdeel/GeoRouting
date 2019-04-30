using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Exceptions;
using GeoRouting.AppLayer.Model;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoRouting.AppLayer.Services
{
    public class WaysService : IWaysService
    {
        public async Task<WayAttributesDTO> GetWaysAttributes(int wayId)
        {
            using (var db = new DbContext())
            {
                var way = await db.Ways.FirstOrDefaultAsync(w => w.Id == wayId);
                
                if (way == null)
                {
                    throw new BadInputException(103, "way with this id doesn't exist");
                }

                var parameters = new
                {
                    way_id = new DataParameter { Value = wayId}
                };

                var pointAttributes = await db.QueryToListAsync<PointAttributeDTO>(@"select t2.id, t2.user as UserId, commentary, category, radius, st_x(location) as longitude, st_y(location) as latitude from 
                                                                                (select attributes.id, attributes.user, commentary, category from
                                                                                (select id_attribute from ways_attributes
                                                                                 where id_way = @way_id) as t1
                                                                                inner join attributes on id_attribute = attributes.id where is_point = true) as t2
                                                                                inner join point_attributes on t2.id = point_attributes.id", parameters);

                var longAttributes = await db.WaysAttributes
                                             .LoadWith(wa => wa.Attribute)
                                             .Where(wa => wa.WayId == wayId)
                                             .Select(wa => new LongAttrbiuteDTO
                                             {
                                                 Id = wa.Attribute.Id,
                                                 UserId = wa.Attribute.UserId,
                                                 Commentary = wa.Attribute.Commentary,
                                                 Category = wa.Attribute.CategoryId
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
    }
}
