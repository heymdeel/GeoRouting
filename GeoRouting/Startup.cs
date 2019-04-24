using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GeoRouting.AppLayer.Config;
using GeoRouting.AppLayer.Services;
using GeoRouting.Middlewares;
using GeoRouting.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace GeoRouting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            services.AddOptions();
            services.Configure<AuthOptions>(Configuration.GetSection("Authentication").GetSection("JWTBearer"));
            
            // Authorization
            services.AddTokenAuthorization(Configuration);

            // Services
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IRoutingService, RoutingService>();
            services.AddTransient<IWaysService, WaysService>();
            services.AddTransient<IAttributeService, AttributeService>();

            // MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Automapper
            services.AddAutoMapper();

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "FindMe API", Version = "v1.0" });
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "FindMe.xml"));

                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FindMe API V1");
                });
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseMiddleware<AppErrorsMiddleware>();

            app.UseMvc();
        }
    }
}
