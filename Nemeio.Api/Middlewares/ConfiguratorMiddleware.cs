using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MvvmCross.Platform;
using Nemeio.Core.Services;

namespace Nemeio.Api.Middlewares
{
    public class ConfiguratorMiddleware
    {
        private const string ApiStartFragment = "/api";
        private const string WebsiteIndexFilename = "index.html";

        private readonly RequestDelegate _next;

        public ConfiguratorMiddleware(RequestDelegate next) => this._next = next;

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments(ApiStartFragment))
            {
                //  Is website call
                await LoadWebsite(context);
                return;
            }

            await this._next.Invoke(context);

        }

        private async Task LoadWebsite(HttpContext context)
        {
            var requestPath = context.Request.Path;
            var documentService = Mvx.Resolve<IDocument>();
            var configuratorFolderPath = documentService.GetConfiguratorPath();
            var configuratorPath = Path.Combine(configuratorFolderPath, WebsiteIndexFilename);

            context.Response.StatusCode = 200;

            if (IsIndexRequest(requestPath))
            {
                using (var fileReader = new StreamReader(configuratorPath))
                {
                    await context.Response.WriteAsync(fileReader.ReadToEnd());
                }
            }
            else
            { 
                //  Check if file ressource exists
                var ressourcePath = configuratorFolderPath + requestPath;
                if (File.Exists(ressourcePath))
                {
                    await context.Response.SendFileAsync(ressourcePath);
                }
                else
                {
                    await this._next.Invoke(context);
                }
            }
        }

        private bool IsIndexRequest(string path) => path.Equals("/") || path.Equals($"/{WebsiteIndexFilename}");
    }
}
