using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model.Entities
{
    [Table(Schema = "public", Name = "attribute_rates")]
    public class AttributeRates
    {
        [Column(@"id_user"), PrimaryKey(1), NotNull] public int UserId { get; set; } // integer
        [Column(@"id_attribute"), PrimaryKey(2), NotNull] public int AttributeId { get; set; } // integer
        [Column(@"rate"), NotNull] public int Rate { get; set; } // integer

        [Association(ThisKey = "AttributeId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "attribute_rates_id_attribute_fkey", BackReferenceName = "attributeratesidattributefkeys")]
        public Attribute Attribute { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "attribute_rates_id_user_fkey", BackReferenceName = "attributeratesiduserfkeys")]
        public User User { get; set; }
    }
}
