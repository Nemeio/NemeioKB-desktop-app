using System.Threading.Tasks;
using Nemeio.Core.JsonModels;

namespace Nemeio.Core.Services
{
    public interface INemeioHttpService
    {
        IWebServer WebServer { get; }
        void StartListenToRequests();
        Task StopListeningToRequestsAsync();
    }
}
