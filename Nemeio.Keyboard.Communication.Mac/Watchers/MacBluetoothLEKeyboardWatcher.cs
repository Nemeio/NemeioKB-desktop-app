using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreBluetooth;
using Microsoft.Extensions.Logging;
using Nemeio.Core;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Keyboard.Communication.Mac.Adapters;
using Nemeio.Keyboard.Communication.Mac.Utils;
using Nemeio.Keyboard.Communication.Watchers;

namespace Nemeio.Keyboard.Communication.Mac.Watchers
{
    public class MacBluetoothLEKeyboardWatcher : KeyboardWatcher
    {
        private sealed class PeripheralHolder
        {
            public CBPeripheral Peripheral { get; private set; }
            public Core.Keyboard.Keyboard Keyboard { get; private set; }

            public PeripheralHolder(CBPeripheral peripheral, Core.Keyboard.Keyboard keyboard)
            {
                Peripheral = peripheral ?? throw new ArgumentNullException(nameof(peripheral));
                Keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
            }
        }

        private static readonly int _defaultBleDetectionTimer = 5000;

        public static readonly CBUUID BleDeviceInformationServiceCbuuid = CBUUID.FromString("0000180A-0000-1000-8000-00805F9B34FB");
        public static readonly CBUUID BleManufacturerNameStringCharacteristicCbuuid = CBUUID.FromPartial(0x2A29);
        public static readonly CBUUID BleSoftwareRevisionStringCharacteristicCbuuid = CBUUID.FromPartial(0x2A28);

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly MacBleCapabilityChecker _bleCapability;

        private Thread _detectionThread;
        private bool _detectionThreadRunning;
        private readonly object _detectionThreadStopLock = new object();

        private readonly CBCentralManager _manager;

        private readonly SemaphoreSlim _inUseSemaphore = new SemaphoreSlim(1, 1);

        private readonly IList<PeripheralHolder> _addedDevices;

        public MacBluetoothLEKeyboardWatcher(ILoggerFactory loggerFactory, IKeyboardVersionParser versionParser)
            : base(versionParser)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<MacBluetoothLEKeyboardWatcher>();

            _manager = new CBCentralManager();
            _manager.DisconnectedPeripheral += Manager_DisconnectedPeripheral;
            _addedDevices = new List<PeripheralHolder>();
            _bleCapability = new MacBleCapabilityChecker(loggerFactory, _manager);

            Start();
        }

        public override void Dispose()
        {
            Stop();

            _manager.DisconnectedPeripheral -= Manager_DisconnectedPeripheral;
            _inUseSemaphore.WaitAsync();
            _detectionThread.Abort(0);
        }

        #region Lifecycle

        private void Start()
        {
            if (!_bleCapability.IsAvailable)
            {
                _logger.LogInformation("MacBluetoothLEKeyboardWatcher: No Bluetooth Capability found. Start aborted");
                return;
            }

            _bleCapability.ConnectivityChanged += BleCapability_ConnectivityChanged;
            _bleCapability.Start();

            if (_bleCapability.IsActivated)
            {
                StartDetectionThread();
            }
        }

        private void Stop()
        {
            _bleCapability.ConnectivityChanged -= BleCapability_ConnectivityChanged;
            _bleCapability.Stop();

            ForceDisconnectEveryPeripheral();
            StopDetectionThread();
        }

        private void ForceDisconnectEveryPeripheral()
        {
            //  Force disconnect on each added devices
            if (_addedDevices != null && _addedDevices.Any())
            {
                foreach (var device in _addedDevices)
                {
                    var keyboard = device.Keyboard;

                    RemoveKeyboard(keyboard);
                }

                _addedDevices.Clear();
            }
        }

        #endregion

        #region Thread management

        private void StartDetectionThread()
        {
            lock (_detectionThreadStopLock)
            {
                if (_detectionThreadRunning)
                {
                    _logger.LogTrace("MacBluetoothLEKeyboardWatcher.StartDetectionThread() already running");
                    return;
                }

                _detectionThread = new Thread(KeyboardDetectionThread)
                {
                    Name = "MacBluetoothLEKeyboardWatcher:KeyboardDetectionThread()"
                };
                _detectionThread.Start();
                _detectionThreadRunning = true;
            }
        }

        private void StopDetectionThread()
        {
            lock (_detectionThreadStopLock)
            {
                if (!_detectionThreadRunning)
                {
                    _logger.LogTrace("MacBluetoothLEKeyboardWatcher.StopDetectionThread() already stopped");
                    return;
                }
                _detectionThreadRunning = false;
            }
        }

        private void KeyboardDetectionThread()
        {
            var scanTime = new TimeSpan(0, 0, 5);

            while (_detectionThreadRunning)
            {
                var listTask = ListNemeioDevicesAsync(new CancellationTokenSource(scanTime).Token);

                Task.WhenAll(
                    Task.WhenAny(listTask, Task.Delay(scanTime)),
                    Task.Delay(scanTime)
                ).Wait();
            }
        }

        #endregion

        #region Events

        private void BleCapability_ConnectivityChanged(object sender, EventArgs e)
        {
            if (_bleCapability.IsActivated)
            {
                StartDetectionThread();
            }
            else
            {
                StopDetectionThread();
                ForceDisconnectEveryPeripheral();
            }
        }

        private void Manager_DisconnectedPeripheral(object sender, CBPeripheralErrorEventArgs e)
        {
            if (IsKnownPeripheral(e.Peripheral))
            {
                var holder = _addedDevices.FirstOrDefault(x => x.Peripheral.Identifier.Equals(e.Peripheral.Identifier));
                if (holder != null)
                {
                    RemoveKeyboard(holder.Keyboard);
                }
            }
        }

        #endregion

        /// <summary>
        /// Method to list BLE devices which provide in "Device Information" service
        /// the "Manufacturer Name string" characteristic set to ManufacturerName
        /// </summary>
        private async Task ListNemeioDevicesAsync(CancellationToken cancelToken)
        {
            await _inUseSemaphore.WaitAsync();

            try
            {
                _logger.LogTrace($"ListNemeioDevicesAsync <TaskId:{Task.CurrentId}>");

                var devices = GetPotentialNemeioKeyboards();

                await devices.ForEachAsync(async device =>
                {
                    //  WARNING! Here it's necessary to not discover again services and characteristic for a known device
                    //  If we do that again all objects will be replaced and pending communication can fail

                    if (!_detectionThreadRunning || cancelToken.IsCancellationRequested || IsKnownPeripheral(device))
                    {
                        return;
                    }

                    // study current peripheral properties
                    using (var checker = new MacBleNemeioDeviceChecker(_loggerFactory, _versionParser, _manager, device))
                    {
                        var keyboard = await checker.GetMacBleDeviceIfManufacturerValid();
                        if (keyboard != null)
                        {
                            var holder = new PeripheralHolder(device, keyboard);

                            _addedDevices.Add(holder);

                            AddKeyboard(keyboard);
                        }
                    }
                });
            }
            catch (AggregateException aggregateException)
            {
                _logger.LogError($"ListNemeioDevicesAsync AggregateException <count:{aggregateException.InnerExceptions.Count}>");
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    _logger.LogError(exception, "ListNemeioDevicesAsync");
                }
            }
            finally
            {
                _inUseSemaphore.Release();
            }
        }

        private bool IsKnownPeripheral(CBPeripheral peripheral)
        {
            if (peripheral == null)
            {
                throw new ArgumentNullException(nameof(peripheral));
            }

            var identifier = peripheral.Identifier.ToString();
            var known = Keyboards
                .Select(keyboard => keyboard.Identifier)
                .FirstOrDefault(id => id == identifier);

            return known != null;
        }

        private IEnumerable<CBPeripheral> GetPotentialNemeioKeyboards()
        {
            var peripherals = _manager.RetrieveConnectedPeripherals(
                MacBluetoothLEAdapter.BleNordicUartServiceCbuuid,
                BleDeviceInformationServiceCbuuid
            );

            return peripherals;
        }
    }
}
