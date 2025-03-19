using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Api;
using Nemeio.Core;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Services;
using Nemeio.Core.Settings;

namespace Nemeio.Acl.HttpComm
{
    public class NemeioHttpService : INemeioHttpService
    {
        private readonly IHttpService _httpService;
        private readonly ILogger _logger;

        public IWebServer WebServer { get; private set; }

        public NemeioHttpService(ILoggerFactory loggerFactory, IHttpService httpService, ISettingsHolder settings)
        {
            _httpService = httpService;

            _logger = loggerFactory.CreateLogger<NemeioHttpService>();

            WebServer = new WebServer(loggerFactory, settings);
        }

        public void StartListenToRequests()
        {
            _logger.LogInformation("NemeioHttpService.StartListenToRequests");

            WebServer.Start();
        }

        public async Task StopListeningToRequestsAsync()
        {
            _logger.LogInformation("NemeioHttpService.StopListeningToRequests");

            await WebServer.StopAsync();
        }

        public async Task<UpdateModel> CheckForUpdates() => await _httpService.SendAsync<UpdateModel>(NemeioConstants.UpdatesUrl, HttpMethod.Get, null, null, true);
    }
}
