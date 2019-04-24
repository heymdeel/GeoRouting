using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoRouting.ViewModels
{
    internal class TokenVM
    {
        [JsonProperty("user_id")]
        public int Id { get; set; }

        [JsonProperty("roles")]
        public string[] Roles { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
