using System;
using System.Threading.Tasks;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Services;

namespace Nemeio.Api.Test.Fakes
{
    public class FakeNemeioHttpService : INemeioHttpService
    {
        public IWebServer WebServer { get; set; }

        public Task<UpdateModel> CheckForUpdates()
        {
            throw new NotImplementedException();
        }

        public void StartListenToRequests()
        {
            throw new NotImplementedException();
        }

        public Task StopListeningToRequestsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
