using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.Model.Entities
{
    [Table(Schema = "public", Name = "users")]
    public partial class User
    {
        [Column(@"id"), PrimaryKey, Identity] public int Id { get; set; } // integer
        [Column(@"email"), NotNull] public string Email { get; set; } // character varying(50)
        [Column(@"hash"), NotNull] public string Hash { get; set; } // text
        [Column(@"date_of_registration"), NotNull] public DateTime DateOfRegistration { get; set; } // date
        [Column(@"roles"), NotNull] public string[] Roles { get; set; } // ARRAY

        [Association(ThisKey = "Id", OtherKey = "IdUser", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<Attribute> Attributes { get; set; }
    }
}
