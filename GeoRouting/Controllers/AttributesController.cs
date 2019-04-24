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
    }
}