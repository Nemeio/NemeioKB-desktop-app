using Microsoft.AspNetCore.Builder;
using Nemeio.Api.Middlewares;

namespace Nemeio.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseConfiguratorMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ConfiguratorMiddleware>();

            return applicationBuilder;
        }

        public static IApplicationBuilder UseSwaggerMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<SwaggerMiddleware>();

            return applicationBuilder;
        }
    }
}
