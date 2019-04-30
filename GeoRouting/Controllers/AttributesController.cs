using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    public class AttributesController : ControllerBase
    {
        private readonly IAttributeService attributeService;
        private readonly IMapper mapper;

        public AttributesController(IAttributeService attributeService, IMapper mapper)
        {
            this.attributeService = attributeService;
            this.mapper = mapper;
        }

        // GET: api/attributes/categories
        /// <summary> Get list of attrbiutes categories </summary>
        /// <response code="200"> list of categories </response>
        /// <response code="401"> unauthorized </response>
        [HttpGet("attributes/categories")]
        [Authorize(Roles = "client")]
        [ProducesResponseType(typeof(IEnumerable<CategoryVM>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> GetAttributeCategories()
        {
            var categories = await attributeService.GetCategories();
            var categoriesVM = mapper.Map<IEnumerable<CategoryVM>>(categories);

            return Ok(categoriesVM);
        }

        // POST: api/attributes/point
        /// <summary> Add point attribute </summary>
        /// <response code="201"> created attribute </response>
        /// <response code="400"> bad model validation(101), user doesn't exist(102) </response>
        /// <response code="401"> unauthorized </response>
        [HttpPost("point_attributes")]
        [Authorize(Roles = "client")]
        [ProducesResponseType(typeof(CreatedAttributeVM), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> AddPointAttribute([FromBody] PointAttributeInput attributeInput)
        {
            if (!ModelState.IsValid)
            {
                string errors = JsonConvert.SerializeObject(ModelState.Values
                                .SelectMany(state => state.Errors)
                                .Select(error => error.ErrorMessage));

                throw new BadInputException(101, errors);
            }

            int userId = User.GetUserId();
            var attribute = await attributeService.AddPointAttribute(userId, attributeInput);
            var attributeVM = mapper.Map<CreatedAttributeVM>(attribute);

            return Created($"/api/point_attributes/{attribute.Id}", attributeVM);
        }

        // POST: api/long_attributes
        /// <summary> Add long attribute </summary>
        /// <response code="201"> created attribute </response>
        /// <response code="400"> bad model validation(101), user doesn't exist(102) </response>
        /// <response code="401"> unauthorized </response>
        [HttpPost("long_attributes")]
        [Authorize(Roles = "client")]
        [ProducesResponseType(typeof(CreatedAttributeVM), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> AddLongAttribute([FromBody] LongAttributeInput attributeInput)
        {
            if (!ModelState.IsValid)
            {
                string errors = JsonConvert.SerializeObject(ModelState.Values
                                .SelectMany(state => state.Errors)
                                .Select(error => error.ErrorMessage));

                throw new BadInputException(101, errors);
            }

            int userId = User.GetUserId();
            var attribute = await attributeService.AddLongAttribute(userId, attributeInput);
            var attributeVM = mapper.Map<CreatedAttributeVM>(attribute);

            return Created($"/api/long_attributes/{attribute.Id}", attributeVM);
        }

        // GET: api/attributes/my
        /// <summary> Get user attributes </summary>
        /// <response code="200"> list of attributes</response>
        /// <response code="401"> unauthorized </response>
        [HttpGet("attributes/my")]
        [Authorize(Roles = "client")]
        [ProducesResponseType(typeof(WayAttributesDTO), 200)]
        public async Task<IActionResult> GetUserattributes()
        {
            int userId = User.GetUserId();

            var result = await attributeService.GetUserAttributes(userId);

            return Ok(result);
        }

        // DELETE: api/attributes/{id}
        /// <summary> Remove attribute by id </summary>
        /// <response code="204"> attribute was sucessfully removed </response>
        /// <response code="400"> attribute was not found(105) </response>
        /// <response code="401"> unauthorized </response>
        /// <response code="403"> Access denied: user didn't add that attribute </response>
        [HttpDelete("attributes/{id:int}")]
        [Authorize(Roles = "client")]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        public async Task<IActionResult> RemoveAttribute([FromRoute]int id)
        {
            int userId = User.GetUserId();

            await attributeService.RemoveAttribute(id, userId);

            return NoContent();
        }
    }
}