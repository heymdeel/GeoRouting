using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class PointAttributeDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("commentary")]
        public string Commentary { get; set; }

        [JsonProperty("category")]
        public int Category { get; set; }

        [JsonProperty("radius")]
        public double Radius { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }
    }
}
