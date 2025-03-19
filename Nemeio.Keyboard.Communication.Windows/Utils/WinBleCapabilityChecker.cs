using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Windows.Devices.Radios;

namespace Nemeio.Keyboard.Communication.Windows.Utils
{
    public class WinBleCapabilityChecker
    {
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _radioCheckerReady = new SemaphoreSlim(0);

        private WinBleStateChecker _bleStateChecker;
        private Radio _radio;

        public event EventHandler<EventArgs> ConnectivityChanged;

        public bool IsAvailable
        {
            get
            {
                _radioCheckerReady.Wait();
                bool answer = (_radio != null);
                _radioCheckerReady.Release();
                return answer;
            }
        }

        public bool IsActivated
        {
            get
            {
                if (!IsAvailable)
                {
                    return false;
                }

                return _radio.State == RadioState.On;
            }
        }

        public WinBleCapabilityChecker(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WinBleCapabilityChecker>();

            Task.Run(async () =>
            {
                var radios = await Radio.GetRadiosAsync();
                _radio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
                _radioCheckerReady.Release();
            });
        }

        public void Start()
        {
            if (!IsAvailable)
            {
                return;
            }

            _logger.LogInformation($"WinBleCapabilityChecker.Start(): Radio <Name:{_radio.Name}> <Status:{_radio.State}>");
            _bleStateChecker = new WinBleStateChecker(_radio, RadioStateChanged);

            _bleStateChecker.Start();
        }

        public void Stop()
        {
            _logger.LogInformation("WinBleCapabilityChecker.Stop()");
            _bleStateChecker?.Stop();
        }

        private void RadioStateChanged()
        {
            _logger.LogInformation($"WinBleCapabilityChecker.RadioStateChanged(): Radio <Name:{_radio.Name}> <Status:{_radio.State}>");
            ConnectivityChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
