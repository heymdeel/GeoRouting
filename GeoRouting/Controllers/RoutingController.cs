using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Exceptions;
using GeoRouting.AppLayer.Services;
using GeoRouting.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GeoRouting.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    public class RoutingController : ControllerBase
    {
        private readonly IRoutingService routingService;

        public RoutingController(IRoutingService routingService)
        {
            this.routingService = routingService;
        }

        // POST: api/calculate_route
        /// <summary> Calculate route from one point to another </summary>
        /// <response code="200"> route and attributes </response>
        /// <response code="400"> errors in model validation(101) </response>
        /// <response code="401"> unauthorized </response>
        [HttpPost("calculate_route")]
        [Authorize(Roles = "client")]
        [ProducesResponseType(typeof(IEnumerable<EdgeDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> CalculateRoute([FromBody]CalculateRouteInput routeInput)
        {
            if (!ModelState.IsValid)
            {
                string errors = JsonConvert.SerializeObject(ModelState.Values
                                .SelectMany(state => state.Errors)
                                .Select(error => error.ErrorMessage));

                throw new BadInputException(101, errors);
            }

            var edges = await routingService.CalculateRouteAsync(routeInput);
            
            return Ok(edges);
        }

        
        /// <summary> kudasov pidaras </summary>
        /// <param name="route"></param>
        [HttpPost("route")]
        public async Task<IActionResult> AddRoute([FromBody] List<RoutePointsInput> route)
        {
            await routingService.AddRoute(route);
            return Ok();
        }
    }
}