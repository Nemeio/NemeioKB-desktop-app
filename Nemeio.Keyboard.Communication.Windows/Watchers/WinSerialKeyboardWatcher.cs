using System;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Threading;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Core.Tools.Retry;
using Nemeio.Keyboard.Communication.Adapters;
using Nemeio.Keyboard.Communication.Watchers;
using Nemeio.Keyboard.Communication.Windows.Utils;
using CommKeyboard = Nemeio.Core.Keyboard.Keyboard;

namespace Nemeio.Keyboard.Communication.Windows.Watchers
{
    public class WinSerialKeyboardWatcher : KeyboardWatcher
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IRetryHandler _retryHandler;

        private ManagementEventWatcher _watcherAdded;
        private ManagementEventWatcher _watcherRemoved;

        private static readonly string VendorId = NemeioConstants.VendorId.ToString("X4");
        private static readonly string ProductIdWithInstaller = NemeioConstants.ProductIdWithInstaller.ToString("X4");
        private static readonly string ProductIdWithoutInstaller = NemeioConstants.ProductIdWithoutInstaller.ToString("X4");

        private static readonly string WmiQueryString = $"SELECT DeviceID, PNPDeviceID FROM Win32_SerialPort WHERE PNPDeviceID LIKE '%VID_{VendorId}&PID_{ProductIdWithInstaller}%' OR PNPDeviceID LIKE '%VID_{VendorId}&PID_{ProductIdWithoutInstaller}%'";

        private static string WmiEventQuery(string eventType) => $"SELECT * FROM {eventType} WITHIN {0.5} WHERE TargetInstance ISA 'Win32_SerialPort' AND (TargetInstance.PNPDeviceID LIKE '%VID_{VendorId}&PID_{ProductIdWithInstaller}%' OR TargetInstance.PNPDeviceID LIKE '%VID_{VendorId}&PID_{ProductIdWithoutInstaller}%')";

        public WinSerialKeyboardWatcher(ILoggerFactory loggerFactory, IRetryHandler retryHandler, IKeyboardVersionParser versionParser)
            : base(versionParser)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<WinSerialKeyboardWatcher>();

            _retryHandler = retryHandler ?? throw new ArgumentNullException(nameof(retryHandler));

            //  We need to set current thread culture to
            //  avoid a bug with Windows Media Query

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
             
            CheckForDevices();

            _watcherAdded = RegisterWatcher(WmiEventQuery("__InstanceCreationEvent"), OnNemeioPlugged);
            _watcherRemoved = RegisterWatcher(WmiEventQuery("__InstanceDeletionEvent"), OnNemeioUnplugged);
        }

        private void CheckForDevices()
        {
            using (var searcher = new ManagementObjectSearcher(WmiQueryString))
            using (var collection = searcher.Get())
            {
                var devices = collection
                    .Cast<ManagementObject>()
                    .Select(item => 
                    {
                        var id = (string)item["DeviceID"];

                        return BuildKeyboard(id, GetProtocolVersion());

                    });

                devices.ForEach(AddKeyboard);
            }
        }

        #region Callback

        private void OnNemeioPlugged(object sender, EventArrivedEventArgs e)
        {
            _logger.LogDebug("WinSerialKeyboardWatcher.OnNemeioPlugged()");

            var keyboard = BuildKeyboard(e);

            AddKeyboard(keyboard);
        }

        private void OnNemeioUnplugged(object sender, EventArrivedEventArgs e)
        {
            _logger.LogDebug("WinSerialKeyboardWatcher.OnNemeioUnplugged()");

            var keyboard = BuildKeyboard(e);

            RemoveKeyboard(keyboard);
        }

        private CommKeyboard BuildKeyboard(EventArrivedEventArgs e)
        {
            var instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            var deviceIdentifier = instance.Properties["DeviceID"].Value.ToString();

            return BuildKeyboard(deviceIdentifier, GetProtocolVersion());
        }

        private Version GetProtocolVersion()
        {
            var usbVersion = FindUsbDescriptor.GetUsbVersion(VendorId, ProductIdWithInstaller);
            if (usbVersion == null)
            {
                usbVersion = FindUsbDescriptor.GetUsbVersion(VendorId, ProductIdWithoutInstaller);
            }

            return usbVersion != null ? _versionParser.Parse(usbVersion) : new Version("0.0");
        }

        private CommKeyboard BuildKeyboard(string id, Version protocolVersion)
        {
            var serialPortAdapter = new SerialKeyboardIOAdapter(_loggerFactory, _retryHandler);
            var keyboard = new CommKeyboard(id, protocolVersion, CommunicationType.Serial, serialPortAdapter);

            return keyboard;
        }

        #endregion

        #region WMI

        private ManagementEventWatcher RegisterWatcher(string wqlEventQuery, EventArrivedEventHandler eventArrivedEventHandler)
        {
            var watcher = new ManagementEventWatcher();
            watcher.EventArrived += eventArrivedEventHandler;
            watcher.Query = new WqlEventQuery(wqlEventQuery);
            watcher.Start();
            return watcher;
        }

        private void DisposeWatcher(ref ManagementEventWatcher watcher, EventArrivedEventHandler eventArrived)
        {
            if (watcher == null)
            {
                return;
            }

            watcher.EventArrived -= eventArrived;
            watcher.Stop();
            watcher.Dispose();
            watcher = null;
        }

        #endregion

        public override void Dispose()
        {
            DisposeWatcher(ref _watcherAdded, OnNemeioPlugged);
            DisposeWatcher(ref _watcherRemoved, OnNemeioUnplugged);
        }
    }
}
