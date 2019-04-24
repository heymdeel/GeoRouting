using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class WayAttributesDTO
    {
        [JsonProperty("point_attributes")]
        public IEnumerable<PointAttributeDTO> PointAttributes { get; set; }

        [JsonProperty("long_attributes")]
        public IEnumerable<LongAttrbiuteDTO> LongAttributes { get; set; }
    }
}
