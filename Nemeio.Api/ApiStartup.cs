using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvvmCross.Platform;
using Nemeio.Api.Extensions;
using Nemeio.Core;
using Nemeio.Core.Services;
using Swashbuckle.AspNetCore.Swagger;

//  Needed for unit tests
[assembly: InternalsVisibleTo("Nemeio.Api.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Nemeio.Api
{
    public class ApiStartup
    {
        private const string CorsPolicyName = "CorsPolicy";

        public IConfiguration Configuration { get; }

        public ApiStartup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var version = Mvx.Resolve<IInformationService>()?.GetApplicationVersion();

                options.SwaggerDoc(
                    "v1",
                    new Info
                    {
                        Title = $"{NemeioConstants.AppName} Api",
                        Version = version?.ToString() ?? "0.0.0.0",
                    });

                options.CustomOperationIds(operationIdSelector =>
                {
                    // Force OperationId at format {controller}_{action}.
                    var routeValues = operationIdSelector.ActionDescriptor.RouteValues;
                    var controller = routeValues["controller"];
                    var action = routeValues["action"];
                    return $"{controller}_{action}";
                });

                options.DescribeAllEnumsAsStrings();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment environment)
        {
            applicationBuilder.UseCors(options =>
            {
                options.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            });

            applicationBuilder.UseMvc();

            applicationBuilder.UseConfiguratorMiddleware();
            applicationBuilder.UseSwaggerMiddleware();

            // TODO : do not expose swagger on environment != development.
            applicationBuilder.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("./v1/swagger.json", $"{NemeioConstants.AppName} API");
                options.DisplayRequestDuration();
                options.DisplayOperationId();
            });

            applicationBuilder.UseSwagger();
        }
    }
}
