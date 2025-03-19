using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;
using Microsoft.Extensions.Logging;
using Nemeio.Keyboard.Communication.Adapters;

namespace Nemeio.Keyboard.Communication.Mac.Adapters
{
    public class MacBluetoothLEAdapter : IBluetoothLEAdapter
    {
        public static readonly CBUUID BleNordicUartServiceCbuuid = CBUUID.FromString("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");
        public static readonly CBUUID BleNusRxCharacteristicCbuuid = CBUUID.FromString("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
        public static readonly CBUUID BleNusTxCharacteristicCbuuid = CBUUID.FromString("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");

        private readonly ILogger _logger;
        private readonly CBPeripheral _peripheral;
        private readonly List<byte> _receiveData;

        private CBCharacteristic _rx;
        private CBCharacteristic _tx;

        public int BytesToRead => _receiveData.Count;

        public event EventHandler DataReceived;

        public MacBluetoothLEAdapter(ILoggerFactory loggerFactory, CBPeripheral peripheral)
        {
            _logger = loggerFactory.CreateLogger<MacBluetoothLEAdapter>();
            _receiveData = new List<byte>();
            _peripheral = peripheral ?? throw new ArgumentNullException(nameof(peripheral));
        }

        public async Task ConnectAsync(string identifier)
        {
            if (_peripheral.Identifier.ToString() != identifier)
            {
                throw new InvalidOperationException($"Invalid identifier <{identifier}>");
            }

            await Task.Yield();

            _rx = FindCharacteristic(BleNusRxCharacteristicCbuuid);
            _tx = FindCharacteristic(BleNusTxCharacteristicCbuuid);

            _peripheral.UpdatedCharacterteristicValue += Peripheral_UpdatedCharacteristicValue;
            _peripheral.SetNotifyValue(true, _tx);
        }

        public async Task DisconnectAsync()
        {
            await Task.Yield();

            if (_tx != null)
            {
                _peripheral.SetNotifyValue(false, _tx);
            }

            _peripheral.UpdatedCharacterteristicValue -= Peripheral_UpdatedCharacteristicValue;

            _tx = null;
            _rx = null;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            _receiveData.CopyTo(buffer, offset);
            _receiveData.Clear();

            return count;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            var value = buffer
                .Skip(offset)
                .Take(count)
                .ToArray();

            var data = NSData.FromArray(value);

            _peripheral.WriteValue(data, _rx, CBCharacteristicWriteType.WithoutResponse);
        }

        private void Peripheral_UpdatedCharacteristicValue(object sender, CBCharacteristicEventArgs e)
        {
            // Copy data from source buffer
            var data = e.Characteristic.Value;
            var dataBytes = new byte[data.Length];

            Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));

            // Store to internal data holder
            _receiveData.AddRange(dataBytes);

            DataReceived?.Invoke(this, EventArgs.Empty);
        }

        private CBCharacteristic FindCharacteristic(CBUUID characteristicCbuuid)
        {
            foreach (var service in _peripheral.Services)
            {
                if (service.Characteristics != null)
                {
                    foreach (var characteristic in service.Characteristics)
                    {
                        if (characteristic.UUID == characteristicCbuuid)
                        {
                            return characteristic;
                        }
                    }
                }
            }

            return null;
        }
    }
}
