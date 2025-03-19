using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MvvmCross.Platform;
using Nemeio.Core.Settings;

namespace Nemeio.Api.Middlewares
{
    public class SwaggerMiddleware
    {
        private const string SwaggerStartFragment = "/swagger";

        private readonly RequestDelegate _next;

        public SwaggerMiddleware(RequestDelegate next) => this._next = next;

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(SwaggerStartFragment))
            {
                //  Is a swagger call
                //  Check if user is allowed to continue
                var settings = Mvx.Resolve<ISettingsHolder>();
                var swaggerEnable = settings.Settings?.SwaggerEnable.Value;
                if (swaggerEnable.HasValue)
                {
                    if (!swaggerEnable.Value)
                    {
                        //  Developer set 'false' on settings file
                        return;
                    }
                }
                else
                {
                    //  Key not present on settings file
                    return;
                }
            }

            await this._next.Invoke(context);

        }
    }
}
