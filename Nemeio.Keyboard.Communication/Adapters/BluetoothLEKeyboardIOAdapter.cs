using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nemeio.Keyboard.Communication.Adapters
{
    public class BluetoothLEKeyboardIOAdapter : KeyboardIOAdapter
    {
        public static readonly int MaxAttMtu = 244;
        public static readonly Guid BleNordicUartServiceGuid = Guid.Parse("6E400001-B5A3-F393-E0A9-E50E24DCCA9E");
        public static readonly Guid BleNusRxCharacteristicGuid = Guid.Parse("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
        public static readonly Guid BleNusTxCharacteristicGuid = Guid.Parse("6E400003-B5A3-F393-E0A9-E50E24DCCA9E");
        public static readonly int NusServiceDiscoveryRetryTimeout = 1000;
        public static readonly int NusServiceDiscoveryRetryAttempts = 20;

        private readonly ILogger _logger;
        private readonly IBluetoothLEAdapter _bluetoothLEAdapter;

        public override int BytesToRead
        {
            get => _bluetoothLEAdapter.BytesToRead;
            protected set
            {
                // Nothing to do here 
            }
        }

        public BluetoothLEKeyboardIOAdapter(ILoggerFactory loggerFactory, IBluetoothLEAdapter bluetoothLEAdapter)
            : base(loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BluetoothLEKeyboardIOAdapter>();
            _bluetoothLEAdapter = bluetoothLEAdapter ?? throw new ArgumentNullException(nameof(bluetoothLEAdapter));
            _bluetoothLEAdapter.DataReceived += BluetoothLEAdapter_DataReceived;
        }

        private void BluetoothLEAdapter_DataReceived(object sender, EventArgs e) => RaiseOnDataRecevied();

        public override Task ConnectAsync(string identifier) => _bluetoothLEAdapter.ConnectAsync(identifier);

        public override Task DisconnectAsync() => _bluetoothLEAdapter.DisconnectAsync();

        public override int Read(byte[] buffer, int offset, int count) => _bluetoothLEAdapter.Read(buffer, offset, count);

        public override void Write(byte[] buffer, int offset, int count)
        {
            var curOffset = 0;
            var remaining = buffer.Length;

            // split message into chunks of size MaxAttMtu at max
            while (remaining != 0)
            {
                // next packet size
                var curSize = Math.Min(remaining, MaxAttMtu);

                _bluetoothLEAdapter.Write(buffer, curOffset, curSize);

                // prepare next packet or stop condition
                curOffset += curSize;
                remaining -= curSize;
            }
        }
    }
}
