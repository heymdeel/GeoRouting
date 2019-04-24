using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class CalculateRouteInput
    {
        [Required]
        [JsonProperty("source_location")]
        public LocationDTO SourceLocation { get; set; }

        [Required]
        [JsonProperty("target_location")]
        public LocationDTO TargetLocation { get; set; }
    }
}
