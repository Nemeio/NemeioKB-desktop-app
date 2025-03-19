using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Connectivity;
using Nemeio.Core.Services;

namespace Nemeio.Acl.HttpComm.Connectivity
{
    public class NetworkConnectivityChecker : INetworkConnectivityChecker
    {
        //  TODO: Dealing with restricted countries such as China
        public static double Timeout = new TimeSpan(0, 0, 20).Ticks;
        public string HostName = "8.8.8.8";

        private readonly ILogger _logger;
        private readonly Timer _timer;
        private readonly IHttpService _httpService;

        private bool _internetAvailable;

        public double CheckInterval { get; set; }

        public bool InternetAvailable
        {
            get => _internetAvailable;
            private set
            {
                if (value == _internetAvailable)
                {
                    //  Bypass if value is the same
                    return;
                }

                _logger.LogInformation($"Connectivity changed, from <{_internetAvailable}> to <{value}>");
                _internetAvailable = value;

                ConnectivityStatusChanged?.Invoke(
                    this,
                    new NetworkConnectivityEventArgs(value)
                );
            }
        }

        public event EventHandler<NetworkConnectivityEventArgs> ConnectivityStatusChanged;

        /// <summary>
        /// Check if Internet is available.
        /// I would have preferred to use the "NetworkChange.NetworkAvailabilityChanged" but after several tests it seems that this class does not work as expected
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="httpService"></param>
        public NetworkConnectivityChecker(ILoggerFactory loggerFactory, IHttpService httpService)
        {
            CheckInterval = Timeout;

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<NetworkConnectivityChecker>();
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            
            _timer = new Timer(CheckInterval) { AutoReset = true };
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            Task.Run(CheckInternetConnectivity);
        }

        ~NetworkConnectivityChecker()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= Timer_Elapsed;
            }
        }

        public async Task<bool> InternetIsAvailable()
        {
            try
            {
                await _httpService.Ping(HostName);

                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        private async Task CheckInternetConnectivity() => InternetAvailable = await InternetIsAvailable();

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e) => await CheckInternetConnectivity();
    }
}
