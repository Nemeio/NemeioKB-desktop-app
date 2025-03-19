using System;
using System.Diagnostics;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.EventArguments;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Notifier
{
    public class BatteryNotifier
    {
        private static int TwentySecondsConnectionDelay = 20;

        private IBatteryHolder _batteryHolder;
        protected DateTime _connectionTimerStarted;

        public bool NormalNotificationSended { get; private set; }
        public bool LowNotificationSended { get; private set; }
        public bool VeryLowNotificationSended { get; private set; }
        public bool BatteryVisibility { get; set; }

        public event EventHandler<BatteryInformationsChangedEventArgs> BatteryInformationChanged;
        public event EventHandler<NotificationReleaseEventArgs> NotificationReleased;

        public BatteryNotifier(IBatteryHolder batteryHolder)
        {
            _batteryHolder = batteryHolder;
        }

        public void Start()
        {
            NormalNotificationSended = false;
            LowNotificationSended = false;
            VeryLowNotificationSended = false;

            _connectionTimerStarted = DateTime.Now;
            _batteryHolder.OnBatteryLevelChanged += DeviceController_BatteryChanged;
        }

        public void Stop()
        {
            _batteryHolder.OnBatteryLevelChanged -= DeviceController_BatteryChanged;

            UnplugKeyboard();
        }

        public void UpdateBatteryStatus()
        {
            var batteryLevel = _batteryHolder.Battery.Level;

            //  Before check battery, we verify that keyboard is connected or not
            if (KeyboardIsDisconnected() || (batteryLevel <= NemeioConstants.NemeioMinimumBatteryLevel && !ConnectedForAWhile()))
            {
                UnplugKeyboard();
                return;
            }

            BatteryVisibility = true;
            ResetNotificationIfNeeded();

            if (IsLowBattery() && !IsBatteryCharging())
            {
                StringId messageId;
                var needNotificationRelease = TryGetLowBatteryMessage(out messageId);
                if (needNotificationRelease)
                {
                    RaiseNotificationRelease(messageId);
                }
            }

            RaiseUpdateBatteryLevel(_batteryHolder.Battery);
        }

        private bool KeyboardIsDisconnected() => _batteryHolder == null || _batteryHolder.Battery == null;

        private void ResetNotificationIfNeeded()
        {
            //  Battery level is higher than minimum
            //  Check to reactivate notification for each step

            var batteryLevel = _batteryHolder.Battery.Level;

            if (batteryLevel > NemeioConstants.NemeioMinimumBatteryLevel)
            {
                NormalNotificationSended = false;
            }

            if (batteryLevel > NemeioConstants.NemeioLowLevelBattery)
            {
                LowNotificationSended = false;
            }

            if (batteryLevel > NemeioConstants.NemeioVeryLowLevelBattery)
            {
                VeryLowNotificationSended = false;
            }
        }

        private void DeviceController_BatteryChanged(object sender, EventArgs e)
        {
            UpdateBatteryStatus();
        }

        private bool ConnectedForAWhile()
        {
            var connectForAWhile = (DateTime.Now - _connectionTimerStarted).TotalSeconds > TwentySecondsConnectionDelay;

            Debug.WriteLine($"connectForAWhile: {connectForAWhile}");

            return connectForAWhile;
        }

        private void RaiseUpdateBatteryLevel(BatteryInformation informations)
        {
            BatteryInformationChanged?.Invoke(
                this,
                new BatteryInformationsChangedEventArgs(informations)
            );
        }

        private void RaiseNotificationRelease(StringId id)
        {
            NotificationReleased?.Invoke(
                this,
                new NotificationReleaseEventArgs(id)
            );
        }

        private bool IsLowBattery()
        {
            if (_batteryHolder.Battery != null)
            {
                return _batteryHolder.Battery.Level <= NemeioConstants.NemeioMinimumBatteryLevel;
            }

            return false;
        }

        private bool IsBatteryCharging()
        {
            return !_batteryHolder.Battery.TimeToFull.StandBy;
        }
        private bool TryGetLowBatteryMessage(out StringId message)
        {

            if (_batteryHolder.Battery != null)
            {
                var batteryLevel = _batteryHolder.Battery.Level;

                if (batteryLevel <= NemeioConstants.NemeioVeryLowLevelBattery)
                {
                    message = StringId.BatteryVeryLowLevelMessageNotification;
                    if (VeryLowNotificationSended == false)
                    {
                        VeryLowNotificationSended = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (batteryLevel <= NemeioConstants.NemeioLowLevelBattery)
                {
                    message = StringId.BatteryLowLevelMessageNotification;
                    if (LowNotificationSended == false)
                    {
                        LowNotificationSended = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            message = StringId.BatteryMessageNotification;
            if (NormalNotificationSended == false)
            {
                NormalNotificationSended = true;
                return true;
            }

            return false;
        }

        private void UnplugKeyboard()
        {
            BatteryVisibility = false;
            RaiseUpdateBatteryLevel(_batteryHolder.Battery);
        }
    }
}
