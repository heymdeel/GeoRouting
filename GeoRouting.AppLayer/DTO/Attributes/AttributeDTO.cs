using LinqToDB.Mapping;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class AttributeDTO
    {
        [Column("id_attribute")]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Column("id_way")]
        [JsonIgnore]
        public int EdgeId { get; set; }

        [Column("is_point")]
        [JsonProperty("is_point")]
        public bool IsPoint { get; set; }
    }
}
