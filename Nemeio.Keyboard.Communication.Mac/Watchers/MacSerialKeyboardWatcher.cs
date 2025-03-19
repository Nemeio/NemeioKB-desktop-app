using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Core.Tools.Retry;
using Nemeio.Keyboard.Communication.Adapters;
using Nemeio.Keyboard.Communication.Mac.Utils;
using Nemeio.Keyboard.Communication.Watchers;
using CommKeyboard = Nemeio.Core.Keyboard.Keyboard;

namespace Nemeio.Keyboard.Communication.Mac.Watchers
{
    public class MacSerialKeyboardWatcher : KeyboardWatcher
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IoKitUsbExtern _ioKitUsbExtern;
        private readonly IRetryHandler _retryHandler;

        public MacSerialKeyboardWatcher(ILoggerFactory loggerFactory, IRetryHandler retryHandler, IKeyboardVersionParser versionParser)
            : base(versionParser)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<MacSerialKeyboardWatcher>();
            _retryHandler = retryHandler ?? throw new ArgumentNullException(nameof(retryHandler));
            _ioKitUsbExtern = new IoKitUsbExtern(SuggestDevice, RemoveDevice);

            Task.Run(() =>
            {
                try
                {
                    _ioKitUsbExtern.Start();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Start usb externe failed");
                }
            });
        }

        public override void Dispose()
        {
            _ioKitUsbExtern.Stop();
        }

        private void SuggestDevice(IntPtr refCon, IntPtr iterator)
        {
            _ioKitUsbExtern.Iterate(iterator, (device, usbDeviceString) =>
            {
                try
                {
                    var keyboard = BuildKeyboard(device, usbDeviceString);

                    AddKeyboard(keyboard);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"AddDevice failed");
                }
            });
        }

        private void RemoveDevice(IntPtr refCon, IntPtr iterator)
        {
            //  We ALWAYS must iterate over list to re-arm notification
            _ioKitUsbExtern.Iterate(iterator, null);

            var devices = IoKitUsbExtern.GetConnectedNemeioDevices();

            var missingKeyboards = Keyboards
                .Select(x => x.Identifier)
                .Except(devices.Select(y => y.Identifier))
                .ToList();

            foreach (var missingKeyboardId in missingKeyboards)
            {
                var keyboard = Keyboards.First(x => x.Identifier == missingKeyboardId);

                RemoveKeyboard(keyboard);
            }
        }

        private CommKeyboard BuildKeyboard(IntPtr device, string identifier)
        {
            var stringVersion = IoKitUsbExtern.GetDeviceVersionProperty(device);
            var version = new Version(stringVersion);

            var serialAdapter = new SerialKeyboardIOAdapter(_loggerFactory, _retryHandler);

            return new CommKeyboard(identifier, version, CommunicationType.Serial, serialAdapter);
        }
    }
}
