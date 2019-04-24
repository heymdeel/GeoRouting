using LinqToDB.Mapping;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model.Entities
{
    [Table(Schema = "public", Name = "long_attributes")]
    public partial class LongAttributes
    {
        [Column(@"id"), PrimaryKey, NotNull] public int Id { get; set; } // integer
        [Column(@"source"), NotNull] public Point Source { get; set; } // USER-DEFINED
        [Column(@"target"), NotNull] public Point Target { get; set; } // USER-DEFINED

        [Association(ThisKey = "Id", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.OneToOne, KeyName = "long_attributes_id_fkey", BackReferenceName = "longidfkey")]
        public Attribute Attribute { get; set; }
    }
}
