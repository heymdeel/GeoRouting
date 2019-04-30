using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model.Entities
{
    [Table(Schema = "public", Name = "categories")]
    public partial class Category
    {
        [Column(@"id"), PrimaryKey, Identity] public int Id { get; set; } // integer
        [Column(@"name"), Nullable] public string Name { get; set; } // character varying(30)
        [Column(@"is_point"), NotNull] public bool IsPoint { get; set; } // boolean
        [Column(@"is_long"), NotNull] public bool IsLong { get; set; } // boolean
        [Column(@"coeff"), NotNull] public double Coefficient { get; set; }

        [Association(ThisKey = "Id", OtherKey = "Category", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<Attribute> Attributes { get; set; }
    }
}
