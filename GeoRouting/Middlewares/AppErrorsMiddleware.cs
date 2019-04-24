using GeoRouting.AppLayer.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeoRouting.Middlewares
{
    public class AppErrorsMiddleware
    {
        private readonly RequestDelegate _next;

        public AppErrorsMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (AppLayerException ex)
            {
                int statusCode = (int)HttpStatusCode.InternalServerError;

                var error = new ErrorResponse
                {
                    Code = ex.Code,
                    Message = ex.Message
                };

                string json = JsonConvert.SerializeObject(error);

                if (ex is BadInputException)
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                }

                if (ex is AccessRefusedException)
                {
                    statusCode = (int)HttpStatusCode.Forbidden;
                }

                context.Response.ContentType = "application/json; charset=utf-8";
                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(json);
            }
        }
    }
}
