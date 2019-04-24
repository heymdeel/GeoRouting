using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model.Entities
{
    [Table(Schema = "public", Name = "ways_attributes")]
    public partial class WaysAttributes
    {
        [Column(@"id_attribute"), PrimaryKey(1), NotNull] public int AttributeId { get; set; } // integer
        [Column(@"id_way"), PrimaryKey(2), NotNull] public int WayId { get; set; } // integer
        [Column(@"coverage"), NotNull] public double Coverage { get; set; } // integer


        [Association(ThisKey = "AttributeId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "ways_attributes_id_attribute_fkey", BackReferenceName = "waysidattributefkeys")]
        public Attribute Attribute { get; set; }

        [Association(ThisKey = "WayId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "ways_attributes_id_way_fkey", BackReferenceName = "attributesidwayfkeys")]
        public Way Way { get; set; }
    }
}
