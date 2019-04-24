using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model.Entities
{
    [Table(Schema = "public", Name = "attributes")]
    public class Attribute
    {
        [Column(@"id"), PrimaryKey, Identity] public int Id { get; set; } // integer
        [Column(@"user"), NotNull] public int UserId { get; set; } // integer
        [Column(@"commentary"), Nullable] public string Commentary { get; set; } // character varying(200)
        [Column(@"category"), NotNull] public int CategoryId { get; set; } // integer
        [Column(@"is_point"), NotNull] public bool IsPoint { get; set; } // integer


        [Association(ThisKey = "Id", OtherKey = "AttributeId", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<AttributeRates> AttributeRates { get; set; }
        
        [Association(ThisKey = "Category", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "attributes_category_fkey", BackReferenceName = "attributescategoryfkeys")]
        public Category Category { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "attributes_id_user_fkey", BackReferenceName = "attributesiduserfkeys")]
        public User User { get; set; }

        [Association(ThisKey = "Id", OtherKey = "Id", CanBeNull = true, Relationship = Relationship.OneToOne, IsBackReference = true)]
        public LongAttributes LongAttribute { get; set; }

        [Association(ThisKey = "Id", OtherKey = "Id", CanBeNull = true, Relationship = Relationship.OneToOne, IsBackReference = true)]
        public PointAttribute PointAttribute { get; set; }

        [Association(ThisKey = "Id", OtherKey = "AttributeId", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<WaysAttributes> WaysAttributes { get; set; }
    }
}
