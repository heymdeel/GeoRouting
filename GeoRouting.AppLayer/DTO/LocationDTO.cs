using LinqToDB.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class LocationDTO
    {
        [Required, Range(-180, 180)]
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [Required, Range(-90, 90)]
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
    }
}
