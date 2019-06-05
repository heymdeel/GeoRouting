using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GeoRouting.AppLayer.DTO;
using GeoRouting.AppLayer.Exceptions;
using GeoRouting.AppLayer.Services;
using GeoRouting.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GeoRouting.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IMapper mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            this.authService = authService;
            this.mapper = mapper;
        }

        // POST: api/sing_up
        /// <summary> Sign up user </summary>
        /// <response code="200"> successfull sign up </response>
        /// <response code="400"> errors in model validation(101) or user already exists(100) </response>
        [HttpPost("sign_up")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> SignUpAsync([FromBody]SignUpInput userData)
        {
            if (!ModelState.IsValid)
            {
                string errors = JsonConvert.SerializeObject(ModelState.Values
                                .SelectMany(state => state.Errors)
                                .Select(error => error.ErrorMessage));

                throw new BadInputException(101, errors);
            }

            await authService.SignUpUserAsync(userData);

            return Ok();
        }

        // POST: api/sign_in
        /// <summary> Sign in user </summary>
        /// <response code="200"> token and user's roles </response>
        /// <response code="400"> errors in model validation(101) or user was not found(102) </response>
        [HttpPost("sign_in")]
        [ProducesResponseType(typeof(TokenVM), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> SignInUser([FromBody]SignInInput userData)
        {
            if (!ModelState.IsValid)
            {
                string errors = JsonConvert.SerializeObject(ModelState.Values
                                .SelectMany(state => state.Errors)
                                .Select(error => error.ErrorMessage));

                throw new BadInputException(101, errors);
            }

            var user = await authService.SignInUserAsync(userData);
            string access_token = authService.GenerateToken(user);

            var tokenVM = mapper.Map<TokenVM>((access_token, user));

            return Ok(tokenVM);
        }
    }
}