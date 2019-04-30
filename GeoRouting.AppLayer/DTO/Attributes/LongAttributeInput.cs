using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class LongAttributeInput
    {
        [JsonProperty("commentary")]
        [StringLength(250)]
        public string Commentary { get; set; }

        [JsonProperty("category")]
        [Required]
        public int CategoryId { get; set; }

        [JsonProperty("points")]
        [Required]
        public IEnumerable<LocationDTO> Points { get; set; }
    }
}
