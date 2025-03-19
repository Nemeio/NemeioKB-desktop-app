namespace Nemeio.Core.Connectivity
{
    public class NetworkConnectivityEventArgs
    {
        public bool IsAvailable { get; private set; }

        public NetworkConnectivityEventArgs(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }
    }
}
