using System;
using System.Collections.Generic;
using System.Text;

namespace GeoRouting.AppLayer.DTO
{
    public class RoutePointsInput
    {
        public LocationDTO Location { get; set; }

        public DateTime Time { get; set; }
    }
}
