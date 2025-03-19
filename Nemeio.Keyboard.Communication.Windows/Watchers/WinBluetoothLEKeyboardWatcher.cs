using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Keyboard.Communication.Watchers;
using Nemeio.Keyboard.Communication.Windows.Utils;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace Nemeio.Keyboard.Communication.Windows.Watchers
{
    class WinBluetoothLEKeyboardWatcher : KeyboardWatcher
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly WinBleCapabilityChecker _bleCapabilityChecker;

        private DeviceWatcher _bleDeviceWatcher;

        private IList<DeviceHolder> _addedDevices;

        public WinBluetoothLEKeyboardWatcher(ILoggerFactory loggerFactory, IKeyboardVersionParser versionParser)
            : base(versionParser)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<WinBluetoothLEKeyboardWatcher>();
            _bleCapabilityChecker = new WinBleCapabilityChecker(loggerFactory);
            _addedDevices = new List<DeviceHolder>();

            Run();
        }

        private void Run()
        {
            if (!_bleCapabilityChecker.IsAvailable)
            {
                _logger.LogInformation("WinBleKeyboardListWatcher.Start(): No Bluetooth Capability found. Start aborted");
                
                return;
            }

            _bleCapabilityChecker.ConnectivityChanged += BleCapability_ConnectivityChanged;
            _bleCapabilityChecker.Start();
        }

        public override void Dispose() 
        {
            foreach (var device in _addedDevices.ToList())
            {
                _addedDevices.Remove(device);

                device.Dispose();
            }
        }

        #region Events

        private void BleCapability_ConnectivityChanged(object sender, EventArgs e)
        {
            if (_bleCapabilityChecker.IsActivated)
            {
                _bleDeviceWatcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelector());
                _bleDeviceWatcher.Added += DeviceWatcher_Added;
                _bleDeviceWatcher.Removed += DeviceWatcher_Removed;
                _bleDeviceWatcher.Start();
            }
            else
            {
                if (_bleDeviceWatcher != null)
                {
                    _bleDeviceWatcher.Added -= DeviceWatcher_Added;
                    _bleDeviceWatcher.Removed -= DeviceWatcher_Removed;
                    _bleDeviceWatcher.Stop();
                    _bleDeviceWatcher = null;
                }

                if (_addedDevices.Any())
                {
                    _logger.LogInformation($"Force disconnect BluetoothLE devices");

                    foreach (var holder in _addedDevices.ToList())
                    {
                        _logger.LogInformation($"Force disconnect BluetoothLE devices with id <{holder.Device.DeviceId}>");

                        holder.Device.ConnectionStatusChanged -= BleDevice_ConnectionStatusChanged;

                        _addedDevices.Remove(holder);

                        var keyboard = holder.CreateKeyboard();

                        RemoveKeyboard(keyboard);
                    }
                }
                else
                {
                    _logger.LogInformation($"No devices to disconnect");
                }
            }
        }

        /// <summary>
        /// This call back is called when a BLE type device is added to the windows system, not presuming of its capabilities or whatsoever
        /// So we first store this device as a bluetooth device and attempt to connect to it so that we can^possibly inquire its Nemeio nature
        /// </summary>
        /// <param name="sender">Windows device watcher for BLE devices</param>
        /// <param name="args">Device information</param>
        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            _logger.LogTrace($"WinBluetoothLEKeyboardWatcher find new device {args.Id}");

            if (sender != _bleDeviceWatcher)
            {
                return;
            }

            var bleDevice = await TryGetBluetoothLEDeviceAsync(args.Id);
            if (bleDevice != null)
            {
                _logger.LogInformation($"New BluetoothLE device found <{bleDevice.DeviceId}>");

                var proxy = new WinBluetoothLeDeviceProxy(_loggerFactory, _versionParser, bleDevice);
                var isNemeioKeyboard = await proxy.IsNemeioKeyboardAsync();
                if (isNemeioKeyboard)
                {
                    var deviceHolder = await proxy.CreateDeviceHolderAsync();

                    deviceHolder.Device.ConnectionStatusChanged += BleDevice_ConnectionStatusChanged;

                    _addedDevices.Add(deviceHolder);

                    CheckBluetoothLEDevice(bleDevice);
                }
            }
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            _logger.LogTrace($"WinBluetoothLEKeyboardWatcher device lost {args.Id}");

            if (sender != _bleDeviceWatcher)
            {
                return;
            }

            var bleDevice = await TryGetBluetoothLEDeviceAsync(args.Id);
            if (bleDevice != null)
            {
                var knownDevice = _addedDevices.FirstOrDefault(x => x.Id.Equals(bleDevice.DeviceId));
                if (knownDevice != null)
                {
                    knownDevice.Device.ConnectionStatusChanged -= BleDevice_ConnectionStatusChanged;

                    _addedDevices.Remove(knownDevice);

                    CheckBluetoothLEDevice(bleDevice);
                }
            }
        }

        private void BleDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args) => CheckBluetoothLEDevice(sender);

        #endregion

        private void CheckBluetoothLEDevice(BluetoothLEDevice bluetoothLEDevice)
        {
            if (bluetoothLEDevice == null)
            {
                return;
            }

            var knownDevice = _addedDevices.FirstOrDefault(x => x.Id.Equals(bluetoothLEDevice.DeviceId));
            if (knownDevice != null)
            {
                var keyboard = knownDevice.CreateKeyboard();

                if (bluetoothLEDevice.ConnectionStatus == BluetoothConnectionStatus.Connected)
                {
                    _logger.LogInformation($"Add keyboard <{keyboard.Identifier}> to connected keyboard list");

                    AddKeyboard(keyboard);
                }
                else
                {
                    _logger.LogInformation($"Remove keyboard <{keyboard.Identifier}> from connected keyboard list");

                    RemoveKeyboard(keyboard);
                }
            }
        }

        private async Task<BluetoothLEDevice> TryGetBluetoothLEDeviceAsync(string deviceId)
        {
            BluetoothLEDevice device = null;

            try
            {
                device = await BluetoothLEDevice.FromIdAsync(deviceId);
            }
            catch (ArgumentException exception)
            {
                _logger.LogError(exception, $"Impossible to get bluetooth le device");
            }

            return device;
        }
    }
}
