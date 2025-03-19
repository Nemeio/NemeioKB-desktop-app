using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Keyboard.Communication.Utils;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Nemeio.Keyboard.Communication.Windows.Utils
{
    internal sealed class WinBluetoothLeDeviceProxy
    {
        private static readonly Guid BleDeviceInformationServiceGuid = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb");
        private static readonly Guid BleManufacturerNameStringCharacteristicGuid = Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb");
        private static readonly Guid BleSoftwareRevisionCharacteristicGuid = Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb");

        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IKeyboardVersionParser _versionParser;
        private readonly BluetoothLEDevice _device;

        private string _softwareRevision;

        public string Id => _device.DeviceId;

        public WinBluetoothLeDeviceProxy(ILoggerFactory loggerFactory, IKeyboardVersionParser versionParser, BluetoothLEDevice device)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<WinBluetoothLeDeviceProxy>();
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _versionParser = versionParser ?? throw new ArgumentNullException(nameof(versionParser));
        }

        public async Task<bool> IsNemeioKeyboardAsync()
        {
            try
            {
                var manufacturer = await ReadManufacturerNameStringAsync();
                if (!manufacturer.Equals(NemeioConstants.ManufacturerName))
                {
                    return false;
                }

                _softwareRevision = await ReadSoftwareRevisionStringAsync();
                if (string.IsNullOrEmpty(_softwareRevision))
                {
                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Get device name failed. IsNemeioKeyboard failed <{_device?.DeviceInformation}>");

                return false;
            }
        }

        public Task<string> ReadManufacturerNameStringAsync() => ReadCharacteristicStringValueAsync(BleDeviceInformationServiceGuid, BleManufacturerNameStringCharacteristicGuid);

        public Task<string> ReadSoftwareRevisionStringAsync() => ReadCharacteristicStringValueAsync(BleDeviceInformationServiceGuid, BleSoftwareRevisionCharacteristicGuid);

        public async Task<DeviceHolder> CreateDeviceHolderAsync()
        {
            var revision = await ReadStringOrGetDefaultValueAsync(ReadSoftwareRevisionStringAsync, "0.0");
            var revisionVersion = _versionParser.Parse(revision);

            var holder = new DeviceHolder(_loggerFactory, revisionVersion, _device);

            return holder;
        }

        private async Task<string> ReadCharacteristicStringValueAsync(Guid serviceGuid, Guid characteristicGuid)
        {
            // get "Device Information" service
            var sResult = await _device.GetGattServicesForUuidAsync(serviceGuid, BluetoothCacheMode.Uncached);
            if (sResult == null)
            {
                _logger.LogTrace($"<Device:{_device.DeviceId}> Cannot get {serviceGuid} result");

                throw new InvalidOperationException("Read failed");
            }

            if (sResult.Status != GattCommunicationStatus.Success || !sResult.Services.Any())
            {
                _logger.LogTrace($"<Device:{_device.DeviceId}> {serviceGuid} result <{sResult.Status}>");

                throw new InvalidOperationException("Read failed");
            }

            var service = sResult?.Services?.First();

            var cResult = await service?.GetCharacteristicsForUuidAsync(characteristicGuid, BluetoothCacheMode.Uncached);
            if (cResult == null)
            {
                _logger.LogTrace($"<Device:{_device.DeviceId}> No result when try to get {characteristicGuid} characteristic");

                throw new InvalidOperationException("Read failed");
            }

            if (cResult.Status != GattCommunicationStatus.Success || !cResult.Characteristics.Any())
            {
                _logger.LogTrace($"<Device:{_device.DeviceId}> {characteristicGuid} characteristic Result<{cResult.Status}>");

                throw new InvalidOperationException("Read failed");
            }

            var characteristic = cResult?.Characteristics?.First();
            if (characteristic == null)
            {
                _logger.LogTrace($"<Device:{_device.DeviceId}> Cannot find {characteristicGuid} characteristic>");

                throw new InvalidOperationException("Read failed");
            }

            if (!characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
            {
                _logger.LogTrace($"<Device:{_device.DeviceId}> {characteristicGuid} characteristic has no read capability");

                throw new InvalidOperationException("Read failed");
            }

            var readResult = await characteristic?.ReadValueAsync();
            if (readResult == null)
            {
                _logger.LogTrace($"<Device:{_device.DeviceId}> {characteristicGuid} characteristic has no read capability");

                throw new InvalidOperationException("Read failed");
            }

            if (readResult.Status != GattCommunicationStatus.Success)
            {
                _logger.LogTrace($"<Device:{_device.DeviceId}> {characteristicGuid} characteristic read value async Result<{readResult.Status}>");

                throw new InvalidOperationException("Read failed");
            }

            var reader = DataReader.FromBuffer(readResult.Value);
            var input = new byte[reader.UnconsumedBufferLength];

            reader.ReadBytes(input);

            var characteristicValue = Encoding.UTF8.GetString(input, 0, input.Length);

            return characteristicValue;
        }

        private async Task<string> ReadStringOrGetDefaultValueAsync(Func<Task<string>> readTask, string defaultValue)
        {
            var response = defaultValue;

            try
            {
                response = await readTask();
            } 
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Read task failed, we return default value <{defaultValue}>");
            }

            return response;
        }
    }
}
