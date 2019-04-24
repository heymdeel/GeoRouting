using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class SignUpInput
    {
        [Required, StringLength(30, MinimumLength = 5)]
        [JsonProperty("email")]
        public string EMail { get; set; }

        [Required, StringLength(30, MinimumLength = 5)]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
