using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Core.Tools.Retry;
using Nemeio.Keyboard.Communication.Linux.Adapters;
using Nemeio.Keyboard.Communication.Linux.CommandLine;
using Nemeio.Keyboard.Communication.Linux.Devices;
using Nemeio.Keyboard.Communication.Watchers;
using Usb.Events;

namespace Nemeio.Keyboard.Communication.Linux.Watchers
{
    internal sealed class LinuxSerialKeyboardWatcher : KeyboardWatcher
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRetryHandler _retryHandler;
        private readonly ILogger _logger;
        private readonly IUsbEventWatcher _usbEventWatcher;
        private readonly LsSysClassTtyCommandLineExecutor _ttyCommandLineExecutor;
        private readonly IFileSystem _fileSystem;
        private readonly TtyProvider _ttyProvider;

        private readonly IList<DeviceHolder> _devices;

        public LinuxSerialKeyboardWatcher(ILoggerFactory loggerFactory, IRetryHandler retryHandler, IKeyboardVersionParser versionParser, IFileSystem fileSystem)
            : base(versionParser)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _retryHandler = retryHandler ?? throw new ArgumentNullException(nameof(retryHandler));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

            _logger = loggerFactory.CreateLogger<LinuxSerialKeyboardWatcher>();

            _devices = new List<DeviceHolder>();
            _ttyProvider = new TtyProvider(loggerFactory);

            _usbEventWatcher = new UsbEventWatcher(false);
            _usbEventWatcher.UsbDeviceAdded += UsbEventWatcher_UsbDeviceAdded;
            _usbEventWatcher.UsbDeviceRemoved += UsbEventWatcher_UsbDeviceRemoved;
            _usbEventWatcher.Start(true);
        }

        private void UsbEventWatcher_UsbDeviceRemoved(object sender, UsbDevice e)
        {
            var knownDevice = _devices.FirstOrDefault(x => x.Name.Equals(e.DeviceName));
            if (knownDevice != null)
            {
                var ioAdapter = new LinuxSerialKeyboardIOAdapter(_loggerFactory, _retryHandler);
                var keyboard = new Core.Keyboard.Keyboard(knownDevice.Identifier, knownDevice.Version, CommunicationType.Serial, ioAdapter);

                _logger.LogInformation($"Remove keyboard <{keyboard.Identifier}>");

                _devices.Remove(knownDevice);

                RemoveKeyboard(keyboard);
            }
        }

        private async void UsbEventWatcher_UsbDeviceAdded(object sender, UsbDevice e)
        {
            if (IsNemeioDevice(e))
            {
                var keyboard = await CreateKeyboardAsync(e);
                if (keyboard != null)
                {
                    _logger.LogInformation($"Add keyboard <{keyboard.Identifier}>");

                    var holder = new DeviceHolder(keyboard, e.DeviceName);

                    _devices.Add(holder);

                    AddKeyboard(keyboard);
                }
            }
        }

        private async Task<Core.Keyboard.Keyboard> CreateKeyboardAsync(UsbDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            Core.Keyboard.Keyboard keyboard = null;

            var version = await TryParseKeyboardVersionAsync(device);
            var deviceIdentifier = ComputeKeyboardIdentifier(device);
            if (!string.IsNullOrEmpty(deviceIdentifier))
            {
                var ioAdapter = new LinuxSerialKeyboardIOAdapter(_loggerFactory, _retryHandler);
                keyboard = new Core.Keyboard.Keyboard(deviceIdentifier, version, CommunicationType.Serial, ioAdapter);
            }

            return keyboard;
        }

        private string ComputeKeyboardIdentifier(UsbDevice device)  
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _logger.LogWarning($"Try find identifier for <{device.DeviceName}>");

            const string prefix = "/sys";

            var identifier = string.Empty;

            var ttyDevices = _ttyProvider.GetDevices();
            if (ttyDevices.Any())
            {
                var systemPath = device.DeviceSystemPath.Remove(0, prefix.Length);

                _logger.LogInformation($"SystemPath: <{systemPath}>");

                foreach (var tty in ttyDevices)
                {
                    _logger.LogInformation($"Device: Name=<{tty.Name}> Path=<{tty.Path}>");
                }

                var ttyDevice = ttyDevices
                    .Where(x => x.Path.Contains(systemPath))
                    .FirstOrDefault();

                if (ttyDevice != null)
                {
                    identifier = $"/dev/{ttyDevice.Name}";
                }
                else
                {
                    _logger.LogWarning($"No tty device found");
                }
            }
            else
            {
                _logger.LogWarning($"No tty devices found");
            }

            return identifier;
        }

        private bool IsNemeioDevice(UsbDevice device)
        {
            try
            {
                var vendorId = Convert.ToInt64(device.VendorID, 16);
                var productId = Convert.ToInt64(device.ProductID, 16);

                return vendorId == NemeioConstants.VendorId && (productId == NemeioConstants.ProductIdWithInstaller || productId == NemeioConstants.ProductIdWithoutInstaller);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<Version> ParseKeyboardVersionAsync(UsbDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            const string BcdDeviceFileName = "bcdDevice";

            //  We create file path
            var filePath = Path.Combine(device.DeviceSystemPath, BcdDeviceFileName);

            //  On device directory we try to open file with name "bcdDevice"
            if (!_fileSystem.FileExists(filePath))
            {
                throw new FileNotFoundException($"File <{filePath}> doesn't exists. It's required to know device version");
            }

            var fileContent = await _fileSystem.ReadTextAsync(filePath);
            if (string.IsNullOrEmpty(fileContent))
            {
                throw new InvalidOperationException($"<{BcdDeviceFileName}> content is invalid");
            }

            //  We wait string with 5 digits
            //  e.g. 0302\n

            const int WaitedContentLength = 5;

            if (fileContent.Length != WaitedContentLength)
            {
                throw new InvalidOperationException($"<{BcdDeviceFileName}> with content <{fileContent}> is invalid. It must be only <{WaitedContentLength}> digits");
            }

            const int VersionPartSize = 2;

            var strMajor = fileContent.Substring(0, VersionPartSize);
            var strMinor = fileContent.Substring(VersionPartSize, VersionPartSize);

            int major = 0;
            int minor = 0;

            var majorParsed = int.TryParse(strMajor, out major);
            var minorParsed = int.TryParse(strMinor, out minor);

            if (!majorParsed || !minorParsed)
            {
                throw new InvalidOperationException($"Failed to parse <{BcdDeviceFileName}> file content. <{fileContent}> is invalid");
            }

            var version = new Version(major, minor);

            return version;
        }

        private async Task<Version> TryParseKeyboardVersionAsync(UsbDevice device)
        {
            try
            {
                var version = await ParseKeyboardVersionAsync(device);

                return version;
            }
            catch (Exception)
            {
                return new Version("0.0");
            }
        }

        public override void Dispose()
        {
            if (_usbEventWatcher != null)
            {
                _usbEventWatcher.UsbDeviceAdded -= UsbEventWatcher_UsbDeviceAdded;
                _usbEventWatcher.UsbDeviceRemoved -= UsbEventWatcher_UsbDeviceRemoved;
            }
        }
    }
}
