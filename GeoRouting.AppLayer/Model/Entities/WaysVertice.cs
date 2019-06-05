using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model.Entities
{
    [Table(Schema = "public", Name = "ways_vertices_pgr")]
    public partial class WaysVertice
    {
        [Column(@"id"), PrimaryKey, Identity] public long Id { get; set; } // bigint
        [Column(@"osm_id"), Nullable] public long? OsmId { get; set; } // bigint
        [Column(@"eout"), Nullable] public int? Eout { get; set; } // integer
        [Column(@"lon"), Nullable] public decimal? Longitude { get; set; } // numeric(11,8)
        [Column(@"lat"), Nullable] public decimal? Latitude { get; set; } // numeric(11,8)
        [Column(@"cnt"), Nullable] public int? Cnt { get; set; } // integer
        [Column(@"chk"), Nullable] public int? Chk { get; set; } // integer
        [Column(@"ein"), Nullable] public int? Ein { get; set; } // integer
        [Column(@"the_geom"), Nullable] public object TheGeom { get; set; } // USER-DEFINED

        [Association(ThisKey = "Id", OtherKey = "Source", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<Way> WaysSources { get; set; }

        [Association(ThisKey = "OsmId", OtherKey = "SourceOsm", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<Way> WaysOsmSources { get; set; }

        [Association(ThisKey = "Id", OtherKey = "Target", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<Way> WaysTargets { get; set; }

        [Association(ThisKey = "OsmId", OtherKey = "TargetOsm", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<Way> WaysOsmTargets { get; set; }
    }
}
