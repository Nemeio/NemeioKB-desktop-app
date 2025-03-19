using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    public delegate void WebServerLoaded();

    public interface IWebServer
    {
        int Port { get; }

        string ConfiguratorUrl { get; }

        bool IsSecure { get; }

        void Start();

        Task StopAsync();
    }
}
