using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Model;
using LinqToDB.Data;

namespace GeoRouting.AppLayer.Services
{
    public class RoutingService : IRoutingService
    {
        public async Task<IEnumerable<EdgeDTO>> CalculateRouteAsync(CalculateRouteInput routeInput)
        {
            using (var db = new DbContext())
            {
                var sourceParameters = new
                {
                    lon = new DataParameter { Value = routeInput.SourceLocation.Longitude },
                    lat = new DataParameter { Value = routeInput.SourceLocation.Latitude }
                };

                // defining source point
                var sourcePointId = await db.ExecuteAsync<int>(@"select id from 
	                                                            ((select x1 as x, y1 as y, source as id
	                                                            FROM ways
	                                                            ORDER BY the_geom <-> st_setsrid(st_makepoint(@lon, @lat),4326) limit 1)
	                                                            union
	                                                            (select x2 as x, y2 as y, target as id
	                                                            FROM ways
	                                                            ORDER BY the_geom <-> st_setsrid(st_makepoint(@lon, @lat),4326) limit 1)) as t1
                                                            order by st_setsrid(st_makepoint(x, y),4326) <-> st_setsrid(st_makepoint(@lon, @lat),4326)
                                                            limit 1", sourceParameters);

                // defining target point
                var targetParameters = new
                {
                    lon = new DataParameter { Value = routeInput.TargetLocation.Longitude },
                    lat = new DataParameter { Value = routeInput.TargetLocation.Latitude }
                };

                var targetPointId = await db.ExecuteAsync<int>(@"select id from 
	                                                            ((select x1 as x, y1 as y, source as id
	                                                            FROM ways
	                                                            ORDER BY the_geom <-> st_setsrid(st_makepoint(@lon, @lat),4326) limit 1)
	                                                            union
	                                                            (select x2 as x, y2 as y, target as id
	                                                            FROM ways
	                                                            ORDER BY the_geom <-> st_setsrid(st_makepoint(@lon, @lat),4326) limit 1)) as t1
                                                            order by st_setsrid(st_makepoint(x, y),4326) <-> st_setsrid(st_makepoint(@lon, @lat),4326)
                                                            limit 1", targetParameters);

                var pointsParameters = new
                {
                    source = new DataParameter { Value = sourcePointId },
                    target = new DataParameter { Value = targetPointId }
                };

                // shortest path
                var edges = await db.QueryToListAsync<EdgeDTO>("SELECT x1 as SourceLongitude, y1 as SourceLatitude, x2 as TargetLongitude, y2 as TargetLatitude, id, ways.length_m / (1000 * maxspeed_forward) as time FROM pgr_bdDijkstra('SELECT id, source, target, cost, reverse_cost FROM ways', @source, @target) inner join ways on edge = ways.id", pointsParameters);

                // attributes of shortest path
                var attributes = await db.QueryToListAsync<AttributeDTO>(@"select id_attribute, id_way, is_point from (select id_attribute, id_way from ways_attributes 
	                                                        inner join (SELECT edge FROM pgr_bdDijkstra('SELECT id, source, target, cost, reverse_cost FROM ways', @source, @target) 
	                                                        inner join ways on edge = ways.id) as t1
	                                                        on ways_attributes.id_way = t1.edge) as t2
	                                                        inner join attributes on t2.id_attribute = attributes.id", pointsParameters);

                foreach (var edge in edges)
                {
                    edge.Attributes = attributes.Where(a => a.EdgeId == edge.Id);
                }

                return edges;
            }
        }
    }
}
