using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class LongEdgeDTO
    {
        [JsonProperty("source_long")]
        public double SourceLongitude { get; set; }

        [JsonProperty("source_lat")]
        public double SourceLatitude { get; set; }

        [JsonProperty("target_long")]
        public double TargetLongitude { get; set; }

        [JsonProperty("target_lat")]
        public double TargetLatitude { get; set; }

        [JsonProperty("length")]
        public double Length { get; set; }
    }

    public class LongAttrbiuteDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("commentary")]
        public string Commentary { get; set; }

        [JsonProperty("category")]
        public int Category { get; set; }

        [JsonProperty("edges")]
        public IEnumerable<LongEdgeDTO> Points { get; set; }
    }
}
