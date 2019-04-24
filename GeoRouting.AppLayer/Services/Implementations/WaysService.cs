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
            var wayAttributes = new WayAttributesDTO();

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

                var longAttributes = await db.QueryToListAsync<LongAttrbiuteDTO>(@"select t2.id, t2.user as UserId, commentary, category, st_x(source) as SourceLongitude, st_y(source) as SourceLatitude, st_x(target) as TargetLongitude, st_y(target) as TargetLatitude from 
                                                                                (select attributes.id, attributes.user, commentary, category from
                                                                                (select id_attribute from ways_attributes
                                                                                 where id_way = @way_id) as t1
                                                                                inner join attributes on id_attribute = attributes.id where is_point = false) as t2
                                                                                inner join long_attributes on t2.id = long_attributes.id", parameters);

                wayAttributes.PointAttributes = pointAttributes;
                wayAttributes.LongAttributes = longAttributes;
            }

            return wayAttributes;
        }
    }
}
