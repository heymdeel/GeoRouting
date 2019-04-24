using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoRouting.ViewModels
{
    public class CategoryVM
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("is_point")]
        public bool IsPoint { get; set; }

        [JsonProperty("is_long")]
        public bool IsLong { get; set; }
    }
}
