using System.Collections.Generic;
using Nemeio.Core.Transactions;

namespace Nemeio.Core.Keyboard.Parameters
{
    public class KeyboardParameters : IBackupable<KeyboardParameters>
    {
        public uint InactiveTime { get; set; }
        public uint SleepTime { get; set; }
        public uint InactiveTimeUSBDisconnected { get; set; }
        public uint SleepTimeUSBDisconnected { get; set; }
        public uint PowerOffTimeUSBDisconnected { get; set; }
        public byte LedPowerMaxLevel { get; set; }
        public byte LedPowerInactiveLevel { get; set; }
        public byte BrigthnessStep { get; set; }
        public ushort ButtonLongPressDelay { get; set; }
        public ushort ButtonRepeatLongPressDelay { get; set; }
        public byte CleanRefreshPeriod { get; set; }
        public bool DemoMode { get; set; }
        public ushort LowBatteryBlinkOnDelayMs { get; set; }
        public ushort LowBatteryBlinkOffDelayMs { get; set; }
        public byte LowBatteryLevelThresholdPercent { get; set; }
        public ushort BleBlinkOnDelayMs { get; set; }
        public ushort BleBlinkOffDelayMs { get; set; }
        public bool HighQualityPercent { get; set; }
        public List<byte> BrightnessStepList { get; set; }
        public byte BlackBackgroundCleanRefreshPeriod { get; set; }

        public KeyboardParameters CreateBackup()
        {
            return new KeyboardParameters()
            {
                InactiveTime = InactiveTime,
                SleepTime = SleepTime,
                InactiveTimeUSBDisconnected = InactiveTimeUSBDisconnected,
                SleepTimeUSBDisconnected = SleepTimeUSBDisconnected,
                PowerOffTimeUSBDisconnected = PowerOffTimeUSBDisconnected,
                LedPowerMaxLevel = LedPowerMaxLevel,
                LedPowerInactiveLevel = LedPowerInactiveLevel,
                BrigthnessStep = BrigthnessStep,
                ButtonLongPressDelay = ButtonLongPressDelay,
                ButtonRepeatLongPressDelay = ButtonRepeatLongPressDelay,
                CleanRefreshPeriod = CleanRefreshPeriod,
                DemoMode = DemoMode,
                LowBatteryBlinkOnDelayMs = LowBatteryBlinkOnDelayMs,
                LowBatteryBlinkOffDelayMs = LowBatteryBlinkOffDelayMs,
                BleBlinkOnDelayMs = BleBlinkOnDelayMs,
                BleBlinkOffDelayMs = BleBlinkOffDelayMs,
                HighQualityPercent = HighQualityPercent,
                BrightnessStepList = new List<byte>(BrightnessStepList),
                BlackBackgroundCleanRefreshPeriod = BlackBackgroundCleanRefreshPeriod
            };
        }
    }
}
