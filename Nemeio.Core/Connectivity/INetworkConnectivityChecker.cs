using System.Threading.Tasks;

namespace Nemeio.Core.Connectivity
{
    public interface INetworkConnectivityChecker
    {
        double CheckInterval { get; }

        bool InternetAvailable { get; }

        Task<bool> InternetIsAvailable();
    }
}
