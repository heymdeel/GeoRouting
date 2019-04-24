using LinqToDB.Mapping;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model.Entities
{
    [Table(Schema = "public", Name = "point_attributes")]
    public partial class PointAttribute
    {
        [Column(@"id"), PrimaryKey, NotNull] public int Id { get; set; } // integer
        [Column(@"location"), NotNull] public Point Location { get; set; } // USER-DEFINED
        [Column(@"radius"), NotNull] public double Radius { get; set; } // double precision

        [Association(ThisKey = "Id", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.OneToOne, KeyName = "point_attributes_id_fkey", BackReferenceName = "pointidfkey")]
        public Attribute Attribute { get; set; }
    }
}
