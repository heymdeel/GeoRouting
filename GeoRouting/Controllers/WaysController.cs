using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeoRouting.Controllers
{
    [Route("api/ways")]
    [Produces("application/json")]
    public class WaysController : ControllerBase
    {
        private readonly IWaysService waysService;

        public WaysController(IWaysService waysService)
        {
            this.waysService = waysService;
        }

        // GET: api/ways/{id}/attributes
        /// <summary> Get all attributes of the way with such id </summary>
        /// <response code="200"> point and long attributes </response>
        /// <response code="400"> wrong way id </response>
        /// <response code="401"> unauthorized </response>
        [HttpGet("{wayId:int}/attributes")]
        [Authorize(Roles = "client")]
        [ProducesResponseType(typeof(WayAttributesDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetWayAttributes([FromRoute] int wayId)
        {
            var result = await waysService.GetWaysAttributes(wayId);

            return Ok(result);
        }
    }
}