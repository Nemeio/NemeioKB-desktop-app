using System;
using System.Threading;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Text;
using Nemeio.Core;
using Nemeio.Keyboard.Communication.Mac.Watchers;
using Nemeio.Keyboard.Communication.Mac.Adapters;
using Nemeio.Keyboard.Communication.Adapters;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Utils;

namespace Nemeio.Keyboard.Communication.Mac.Utils
{
    public class MacBleNemeioDeviceChecker : IDisposable
    {
        private sealed class DataHolder
        {
            public string ManufacturerValue { get; set; }
            public string RevisionValue { get; set; }

            public bool IsComplete() => !string.IsNullOrEmpty(ManufacturerValue) && !string.IsNullOrEmpty(RevisionValue);
        }

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly CBCentralManager _manager;
        private readonly CBPeripheral _device;
        private readonly DataHolder _dataHolder;
        private readonly IKeyboardVersionParser _versionParser;

        private SemaphoreSlim _deviceDiscoverySemaphore = new SemaphoreSlim(0, 1);

        public MacBleNemeioDeviceChecker(ILoggerFactory loggerFactory, IKeyboardVersionParser versionParser, CBCentralManager manager, CBPeripheral device)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<MacBleNemeioDeviceChecker>();
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _dataHolder = new DataHolder();
            _versionParser = versionParser ?? throw new ArgumentNullException(nameof(versionParser));

            _manager.ConnectedPeripheral += Manager_ConnectedPeripheral;
            _device.DiscoveredService += Device_DiscoveredService;
            _device.DiscoveredCharacteristic += Device_DiscoveredCharacteristic;
            _device.UpdatedCharacterteristicValue += Device_UpdatedCharacteristicValue;
        }

        public void Dispose()
        {
            _manager.ConnectedPeripheral -= Manager_ConnectedPeripheral;
            _device.DiscoveredService -= Device_DiscoveredService;
            _device.DiscoveredCharacteristic -= Device_DiscoveredCharacteristic;
            _device.UpdatedCharacterteristicValue -= Device_UpdatedCharacteristicValue;
        }

        public async Task<Core.Keyboard.Keyboard> GetMacBleDeviceIfManufacturerValid()
        {
            // start manufacture property inquiry on the connection callback (BLE mechanisms on OSX)
            _manager.ConnectPeripheral(_device);

            await BlockUntilDeviceDiscoveryComplete();

            Core.Keyboard.Keyboard keyboard = null;

            if (_dataHolder.ManufacturerValue.Equals(NemeioConstants.ManufacturerName))
            {
                keyboard = new Core.Keyboard.Keyboard(
                    _device.Identifier.ToString(),
                    _versionParser.Parse(_dataHolder.RevisionValue),
                    CommunicationType.BluetoothLE,
                    new BluetoothLEKeyboardIOAdapter(_loggerFactory, new MacBluetoothLEAdapter(_loggerFactory, _device))
                );
            }

            return keyboard;
        }

        private async Task BlockUntilDeviceDiscoveryComplete() => await _deviceDiscoverySemaphore.WaitAsync();

        private void DeviceDiscoveryComplete() => _deviceDiscoverySemaphore.Release();

        private void Manager_ConnectedPeripheral(object sender, CBPeripheralEventArgs e)
        {
            e.Peripheral.DiscoverServices();
        }

        private void Device_DiscoveredService(object sender, NSErrorEventArgs e)
        {
            if (e.Error != null)
            {
                _logger.LogError($"MacBleKeyboardListWatcher.Device_DiscoveredService() <Error:{e.Error.Description}>");
                return;
            }

            _device.Services.ForEach((service) => _device.DiscoverCharacteristics(service));
        }

        private void Device_DiscoveredCharacteristic(object sender, CBServiceEventArgs e)
        {
            var manufacturerCharacteristic = e.Service.Characteristics.FirstOrDefault(x => x.UUID == MacBluetoothLEKeyboardWatcher.BleManufacturerNameStringCharacteristicCbuuid);
            if (manufacturerCharacteristic != null)
            {
                _device.ReadValue(manufacturerCharacteristic);
            }

            var softwareRevisionCharacteristic = e.Service.Characteristics.FirstOrDefault(x => x.UUID == MacBluetoothLEKeyboardWatcher.BleSoftwareRevisionStringCharacteristicCbuuid);
            if (softwareRevisionCharacteristic != null)
            {
                _device.ReadValue(softwareRevisionCharacteristic);
            }
        }

        private void Device_UpdatedCharacteristicValue(object sender, CBCharacteristicEventArgs e)
        {
            var data = e.Characteristic.Value;
            if (data != null)
            {
                // copy data to regular byte array
                var buffer = new byte[data.Length];
                System.Runtime.InteropServices.Marshal.Copy(data.Bytes, buffer, 0, Convert.ToInt32(data.Length));

                if (e.Characteristic.UUID.Uuid == MacBluetoothLEKeyboardWatcher.BleManufacturerNameStringCharacteristicCbuuid.Uuid)
                {
                    var manufacturer = Encoding.UTF8.GetString(buffer);

                    _logger.LogInformation($"IsNemeioDevice <Device:{_device.Identifier}> <ManufacturerNameString:{manufacturer}>");

                    _dataHolder.ManufacturerValue = manufacturer;
                }
                else if (e.Characteristic.UUID.Uuid == MacBluetoothLEKeyboardWatcher.BleSoftwareRevisionStringCharacteristicCbuuid.Uuid)
                {
                    var revision = Encoding.UTF8.GetString(buffer);

                    _logger.LogInformation($"IsNemeioDevice <Device:{_device.Identifier}> <SoftwareRevision:{revision}>");

                    _dataHolder.RevisionValue = revision;
                }

                if (_dataHolder.IsComplete())
                {
                    DeviceDiscoveryComplete();
                }
            }
        }
    }
}
