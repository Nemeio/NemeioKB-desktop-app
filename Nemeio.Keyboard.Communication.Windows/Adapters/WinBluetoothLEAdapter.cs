using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Keyboard.Communication.Adapters;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;

namespace Nemeio.Keyboard.Communication.Windows.Adapters
{
    public sealed class WinBluetoothLEAdapter : IBluetoothLEAdapter
    {
        private BluetoothLEDevice _bleDevice;
        private List<byte> _receiveData;

        private GattDeviceService _nusService;
        private GattCharacteristic _rx;
        private GattCharacteristic _tx;

        private ILogger _logger;

        public int BytesToRead => _receiveData.Count;

        public event EventHandler DataReceived;

        public WinBluetoothLEAdapter(ILoggerFactory loggerFactory)
        {
            _receiveData = new List<byte>();
            _logger = loggerFactory.CreateLogger<WinBluetoothLEAdapter>();
        }

        public async Task ConnectAsync(string identifier)
        {
            _bleDevice = await BluetoothLEDevice.FromIdAsync(identifier);
            if (_bleDevice == null)
            {
                throw new ArgumentNullException(nameof(_bleDevice));
            }

            await CompleteNusServiceDiscoveryAsync();
        }

        public async Task DisconnectAsync()
        {
            await Task.Yield();

            if (_tx == null)
            {
                return;
            }

            ReleaseNusService();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            _receiveData.CopyTo(buffer, offset);
            _receiveData.Clear();

            return count;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            var iBuffer = buffer.AsBuffer(offset, count);

            _rx.WriteValueAsync(iBuffer, GattWriteOption.WriteWithoutResponse).AsTask().Wait();
        }

        private async Task<bool> CompleteNusServiceDiscoveryAsync()
        {
            var complete = false;
            var retryCount = 0;

            while (!complete)
            {
                complete = await DiscoverNusServiceAsync();

                _logger.LogInformation($"WinBleDeviceAdapater.CompleteNusServiceDiscovery(<success={complete}>)");

                if (!complete)
                {
                    _tx = null;
                    _rx = null;
                    _nusService = null;

                    retryCount += 1;

                    if (retryCount == BluetoothLEKeyboardIOAdapter.NusServiceDiscoveryRetryAttempts)
                    {
                        return false;
                    }

                    Task.Delay(BluetoothLEKeyboardIOAdapter.NusServiceDiscoveryRetryTimeout).Wait();
                }
            }

            return true;
        }

        /// <summary>
        /// Internal method to attach to BLE RX and TX services for Serial NUS communication
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DiscoverNusServiceAsync()
        {
            _logger.LogTrace($"DiscoverNUSService TaskId<{Task.CurrentId}>");

            if (_tx != null)
            {
                // service and characteristics already discovered
                return false;
            }

            // search for specific NUS service from BLE device
            var sResult = await _bleDevice.GetGattServicesForUuidAsync(BluetoothLEKeyboardIOAdapter.BleNordicUartServiceGuid, BluetoothCacheMode.Uncached);
            if (sResult.Status != GattCommunicationStatus.Success || !sResult.Services.Any())
            {
                _logger.LogError($"DiscoverNUSService sResult<{sResult.Status}> TaskId<{Task.CurrentId}>");
                return false;
            }
            _nusService = sResult.Services.First();

            // catch the RX characteristics
            var rxResult = await _nusService.GetCharacteristicsForUuidAsync(BluetoothLEKeyboardIOAdapter.BleNusRxCharacteristicGuid, BluetoothCacheMode.Uncached);
            if (rxResult.Status != GattCommunicationStatus.Success || !rxResult.Characteristics.Any())
            {
                _logger.LogError($"DiscoverNUSService rxResult<{rxResult.Status}> TaskId<{Task.CurrentId}>");
                return false;
            }
            _rx = rxResult.Characteristics.First();

            // catch the TX characteristics
            var txResult = await _nusService.GetCharacteristicsForUuidAsync(BluetoothLEKeyboardIOAdapter.BleNusTxCharacteristicGuid, BluetoothCacheMode.Uncached);
            if (txResult.Status != GattCommunicationStatus.Success || !txResult.Characteristics.Any())
            {
                _logger.LogError($"DiscoverNUSService txResult<{txResult.Status}> TaskId<{Task.CurrentId}>");
                return false;
            }
            _tx = txResult.Characteristics.First();

            // Need to attach to the Notify mechanism on TX
            var status = await _tx.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            if (status != GattCommunicationStatus.Success)
            {
                _logger.LogError($"DiscoverNUSService Tx Notify attachment failed<{status}>");
                return false;
            }
            _tx.ValueChanged += Characteristic_ValueChanged;

            return true;
        }

        private void ReleaseNusService()
        {
            if (_tx != null)
            {
                _tx.ValueChanged -= Characteristic_ValueChanged;
            }

            _rx = null;
            _tx = null;

            // tried to get rid of this dispose but this breaks reconnection mechanism
            // seems that there are known concerns on such items disposal (comment from Edited by Elias Simon Friday, May 10, 2019 8:34 PM )
            // cf. https://social.msdn.microsoft.com/Forums/en-US/1810b32a-ea5f-4183-8f3f-d4a5b76401f9/uwp-ble-gatt-getgattservicesasync-method-returns-gattcommunicationstatusunreachable?forum=wpdevelop
            _nusService?.Dispose();
            _nusService = null;

            _logger.LogInformation("ReleaseNusService done");
        }

        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var received = args.CharacteristicValue.ToArray();

            _receiveData.AddRange(received);

            DataReceived?.Invoke(this, EventArgs.Empty);
        }
    }
}
