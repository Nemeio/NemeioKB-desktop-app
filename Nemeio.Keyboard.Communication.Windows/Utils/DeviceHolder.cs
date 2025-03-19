using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Keyboard.Communication.Adapters;
using Nemeio.Keyboard.Communication.Windows.Adapters;
using Windows.Devices.Bluetooth;
using CommKeyboard = Nemeio.Core.Keyboard.Keyboard;

namespace Nemeio.Keyboard.Communication.Windows.Utils
{
    internal sealed class DeviceHolder : IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;

        public string Id { get; private set; }
        public Version SoftwareRevision { get; private set; }
        public BluetoothLEDevice Device { get; private set; }

        public DeviceHolder(ILoggerFactory loggerFactory, Version softwareRevision, BluetoothLEDevice device)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            SoftwareRevision = softwareRevision ?? throw new ArgumentNullException(nameof(softwareRevision));
            Device = device ?? throw new ArgumentNullException(nameof(device));
            Id = device.DeviceId;
        }

        public CommKeyboard CreateKeyboard()
        {
            var keyboard = new CommKeyboard(
                Id,
                SoftwareRevision,
                CommunicationType.BluetoothLE,
                new BluetoothLEKeyboardIOAdapter(_loggerFactory, new WinBluetoothLEAdapter(_loggerFactory))
            );

            return keyboard;
        }

        public void Dispose() => Device?.Dispose();
    }
}
