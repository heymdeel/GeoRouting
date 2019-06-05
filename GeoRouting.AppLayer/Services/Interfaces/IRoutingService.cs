using GeoRouting.AppLayer.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeoRouting.AppLayer.Services
{
    public interface IRoutingService
    {
        Task<IEnumerable<EdgeDTO>> CalculateRouteAsync(CalculateRouteInput routeInput);
        Task AddRoute(List<RoutePointsInput> route);
    }
}
