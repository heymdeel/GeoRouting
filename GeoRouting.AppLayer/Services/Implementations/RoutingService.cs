using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GeoAPI.Geometries;
using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Model;
using GeoRouting.AppLayer.Model.Entities;
using LinqToDB;
using LinqToDB.Data;
using NetTopologySuite.Geometries;

namespace GeoRouting.AppLayer.Services
{
    public class OldVertice
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Id { get; set; }
    }

    public class RoutingService : IRoutingService
    {
        private readonly IMapper mapper;

        public RoutingService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task AddRoute(List<RoutePointsInput> route)
        {
            using (var db = new DbContext())
            {
                int idSource = -1;
                int idTarget = -1;

                var edges = new List<Way>();
                for (int i = 0; i < route.Count() - 1; i++)
                {
                    var sourcePoint = new Point(route[i].Location.Longitude, route[i].Location.Latitude) { SRID = 4326 };
                    var targetPoint = new Point(route[i + 1].Location.Longitude, route[i + 1].Location.Latitude) { SRID = 4326 };

                    double length = sourcePoint.Distance(targetPoint);
                    double lengthMeters = DistanceInMeters((float)sourcePoint.Y, (float)sourcePoint.X, (float)targetPoint.Y, (float)targetPoint.X);

                    double timeHours = route[i + 1].Time.Subtract(route[i].Time).TotalHours;
                    if (timeHours <= 0)
                    {
                        timeHours = 1 / 3600; // 1 second
                    }

                    double speed = lengthMeters / (1000 * timeHours);

                    if (idSource == -1)
                    {
                        var sourceVertice = new WaysVertice
                        {
                            Longitude = (decimal)sourcePoint.X,
                            Latitude = (decimal)sourcePoint.Y,
                            TheGeom = sourcePoint
                        };

                        idSource = await db.InsertWithInt32IdentityAsync(sourceVertice);
                    }

                    var targetVertice = new WaysVertice
                    {
                        Longitude = (decimal)targetPoint.X,
                        Latitude = (decimal)targetPoint.Y,
                        TheGeom = targetPoint
                    };
                    idTarget = await db.InsertWithInt32IdentityAsync(targetVertice);

                    edges.Add(new Way
                    {
                        Length = length,
                        LengthM = lengthMeters,
                        X1 = sourcePoint.X,
                        Y1 = sourcePoint.Y,
                        X2 = targetPoint.X,
                        Y2 = targetPoint.Y,
                        Source = idSource,
                        Target = idTarget,
                        Cost = length,
                        ReverseCost = length,
                        CostS = (length * 18) / (speed * 5),
                        ReverseCostS = (length * 18) / (speed * 5),
                        Oneway = "NO",
                        OneWay = 2,
                        MaxspeedForward = speed,
                        MaxspeedBackward = speed,
                        TheGeom = new LineString(new Coordinate[] { sourcePoint.Coordinate, targetPoint.Coordinate }) { SRID = 4326 }
                    });

                    idSource = idTarget;
                }

                int n = edges.Count;
                edges.Add(await ConcatEdgeWithTopology(edges[0]));

                edges.Add(await ConcatEdgeWithTopology(edges[n-1]));

                db.BulkCopy(edges);
                db.BulkCopy(mapper.Map<IEnumerable<WayBackUp>>(edges));
            }
        }

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

        private async Task<Way> ConcatEdgeWithTopology(Way newEdge)
        {
            OldVertice closestVertice;
            using (var db = new DbContext())
            {
                var sqlParameters = new
                {
                    lon = new DataParameter { Value = newEdge.X1 },
                    lat = new DataParameter { Value = newEdge.Y1 }
                };

                closestVertice = await db.ExecuteAsync<OldVertice>(@"select id, x as longitude, y as latitude from 
	                                                            ((select x1 as x, y1 as y, source as id
	                                                            FROM ways
	                                                            ORDER BY the_geom <-> st_setsrid(st_makepoint(@lon, @lat),4326) limit 1)
	                                                            union
	                                                            (select x2 as x, y2 as y, target as id
	                                                            FROM ways
	                                                            ORDER BY the_geom <-> st_setsrid(st_makepoint(@lon, @lat),4326) limit 1)) as t1
                                                            order by st_setsrid(st_makepoint(x, y),4326) <-> st_setsrid(st_makepoint(@lon, @lat),4326)
                                                            limit 1", sqlParameters);
            }

            var sourcePoint = new Point(closestVertice.Longitude, closestVertice.Latitude) { SRID = 4326 };
            var targetPoint = new Point(newEdge.X1.Value, newEdge.Y1.Value) { SRID = 4326 };

            double length = sourcePoint.Distance(targetPoint);
            double lengthMeters = DistanceInMeters((float)sourcePoint.Y, (float)sourcePoint.X, (float)targetPoint.Y, (float)targetPoint.X);
            double speed = 5;

            return new Way
            {
                Length = length,
                LengthM = lengthMeters,
                X1 = sourcePoint.X,
                Y1 = sourcePoint.Y,
                X2 = targetPoint.X,
                Y2 = targetPoint.Y,
                Source = closestVertice.Id,
                Target = newEdge.Source,
                Cost = length,
                ReverseCost = length,
                CostS = (length * 18) / (speed * 5),
                ReverseCostS = (length * 18) / (speed * 5),
                Oneway = "NO",
                OneWay = 2,
                MaxspeedForward = speed,
                MaxspeedBackward = speed,
                TheGeom = new LineString(new Coordinate[] { sourcePoint.Coordinate, targetPoint.Coordinate }) { SRID = 4326 }
            };
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private float DistanceInMeters(float lat1, float lng1, float lat2, float lng2)
        {
            double earthRadius = 6371000; //meters
            double dLat = DegreeToRadian(lat2 - lat1);
            double dLng = DegreeToRadian(lng2 - lng1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreeToRadian(lat1)) * Math.Cos(DegreeToRadian(lat2)) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            float dist = (float)(earthRadius * c);

            return dist;
        }
    }
}
