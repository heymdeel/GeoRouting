using LinqToDB.Mapping;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model.Entities
{
    [Table(Schema = "public", Name = "ways")]
    public partial class Way
    {
        [Column(@"id"), PrimaryKey, Identity] public long Id { get; set; } // bigint
        [Column(@"osm_id"), Nullable] public long? OsmId { get; set; } // bigint
        [Column(@"tag_id"), Nullable] public int? TagId { get; set; } // integer
        [Column(@"length"), Nullable] public double? Length { get; set; } // double precision
        [Column(@"length_m"), Nullable] public double? LengthM { get; set; } // double precision
        [Column(@"name"), Nullable] public string Name { get; set; } // text
        [Column(@"source"), Nullable] public long? Source { get; set; } // bigint
        [Column(@"target"), Nullable] public long? Target { get; set; } // bigint
        [Column(@"source_osm"), Nullable] public long? SourceOsm { get; set; } // bigint
        [Column(@"target_osm"), Nullable] public long? TargetOsm { get; set; } // bigint
        [Column(@"cost"), Nullable] public double? Cost { get; set; } // double precision
        [Column(@"reverse_cost"), Nullable] public double? ReverseCost { get; set; } // double precision
        [Column(@"cost_s"), Nullable] public double? CostS { get; set; } // double precision
        [Column(@"reverse_cost_s"), Nullable] public double? ReverseCostS { get; set; } // double precision
        [Column(@"rule"), Nullable] public string Rule { get; set; } // text
        [Column(@"one_way"), Nullable] public int? OneWay { get; set; } // integer
        [Column(@"oneway"), Nullable] public string Oneway { get; set; } // text
        [Column(@"x1"), Nullable] public double? X1 { get; set; } // double precision
        [Column(@"y1"), Nullable] public double? Y1 { get; set; } // double precision
        [Column(@"x2"), Nullable] public double? X2 { get; set; } // double precision
        [Column(@"y2"), Nullable] public double? Y2 { get; set; } // double precision
        [Column(@"maxspeed_forward"), Nullable] public double? MaxspeedForward { get; set; } // double precision
        [Column(@"maxspeed_backward"), Nullable] public double? MaxspeedBackward { get; set; } // double precision
        [Column(@"priority"), Nullable] public double? Priority { get; set; } // double precision
        [Column(@"the_geom"), Nullable] public LineString TheGeom { get; set; } // USER-DEFINED

        [Association(ThisKey = "Id", OtherKey = "WayId", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<WaysAttributes> attributesidwayfkeys { get; set; }
    }
}
