using System;
using CoreBluetooth;
using Microsoft.Extensions.Logging;

namespace Nemeio.Keyboard.Communication.Mac.Utils
{
    public class MacBleCapabilityChecker
    {
        private readonly ILogger _logger;
        private readonly CBCentralManager _manager;

        public event EventHandler<EventArgs> ConnectivityChanged;

        public bool IsAvailable
        {
            get
            {
                if (_manager == null)
                {
                    return false;
                }

                return _manager.State != CBCentralManagerState.Unsupported;
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

                if (_manager == null)
                {
                    return false;
                }

                if (CBCentralManager.Authorization != CBManagerAuthorization.AllowedAlways)
                {
                    _logger.LogWarning($"Application doesn't have Bluetooth permission");

                    return false;
                }

                return _manager.State == CBCentralManagerState.PoweredOn;
            }
        }

        public MacBleCapabilityChecker(ILoggerFactory loggerFactory, CBCentralManager manager)
        {
            _logger = loggerFactory.CreateLogger<MacBleCapabilityChecker>();
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public void Start()
        {
            if (!IsAvailable)
            {
                _logger.LogInformation($"MacBleCapabilityChecker.Start(): No Bluetooth Radio Adapter found.");
                return;
            }

            _logger.LogInformation($"MacBleCapabilityChecker.Start(): Radio <Status:{_manager.State}>");
            _manager.UpdatedState += Manager_UpdatedState;

        }

        public void Stop()
        {
            _logger.LogInformation("MacBleCapabilityChecker.Stop()");
            _manager.UpdatedState -= Manager_UpdatedState;
        }

        private void Manager_UpdatedState(object sender, EventArgs e)
        {
            ConnectivityChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
