using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoRouting.ViewModels
{
    public class CreatedAttributeVM
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("user")]
        public int UserId { get; set; }

        [JsonProperty("commentary")]
        public string Commentary { get; set; }

        [JsonProperty("category")]
        public int CategoryId { get; set; }

        [JsonProperty("is_point")]
        public bool IsPoint { get; set; } 
    }
}
