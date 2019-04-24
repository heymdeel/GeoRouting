using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class PointAttributeInput
    {
        [JsonProperty("commentary")]
        [StringLength(250)]
        public string Commentary { get; set; }

        [JsonProperty("category")]
        [Required]
        public int CategoryId { get; set; }

        [JsonProperty("radius")]
        [Required, Range(10, 100)]
        public double Radius { get; set; }

        [JsonProperty("location")]
        [Required]
        public LocationDTO Location { get; set; }
    }
}
