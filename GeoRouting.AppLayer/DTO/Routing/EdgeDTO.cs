using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class EdgeDTO
    {
        [JsonProperty("edge_id")]
        public int Id { get; set; }

        [JsonProperty("source_longitude")]
        public double SourceLongitude { get; set; }

        [JsonProperty("source_latitude")]
        public double SourceLatitude { get; set; }

        [JsonProperty("target_longitude")]
        public double TargetLongitude { get; set; }

        [JsonProperty("target_latitude")]
        public double TargetLatitude { get; set; }

        [JsonProperty("elapsed_time")]
        public double Time { get; set; }

        [JsonProperty("attributes")]
        public IEnumerable<AttributeDTO> Attributes { get; set; }
    }
}
