using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Nemeio.WinAutoInstaller.EventArgs;

namespace Nemeio.WinAutoInstaller
{
    public class NetworkConnectivityChecker
    {
        //  TODO: Dealing with restricted countries such as China
        private const string HostName       = "www.nemeio.com";
        private const int Timeout           = 500;

        private Timer _timer;

        public event EventHandler<NetworkConnectivityEventArgs> ConnectivityStatusChanged;

        public static async Task<bool> InternetIsAvailable()
        {
            var isAvailable = false;
            using (var client = new HttpClient())
            {
                try
                {
                    //  Force sync
                    var request     = new HttpRequestMessage(HttpMethod.Get, $"https://{HostName}");
                    var response    = await client.SendAsync(request);

                    isAvailable = response.IsSuccessStatusCode;
                }
                catch (HttpRequestException)
                {
                    isAvailable = false;
                }
            }

            return isAvailable;
        }

        public void Start()
        {
            _timer = new Timer(Timeout) { AutoReset = true };
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= Timer_Elapsed;
                _timer = null;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e) => RaiseConnectivityStatusChanged();

        private void RaiseConnectivityStatusChanged()
        {
            Task.Run(async () => 
            {
                var isAvailable = await InternetIsAvailable();

                ConnectivityStatusChanged?.Invoke(
                    this,
                    new NetworkConnectivityEventArgs(isAvailable)
                );
            });
        }
    }
}
