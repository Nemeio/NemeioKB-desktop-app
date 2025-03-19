using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.Parameters.Get
{
    internal sealed class ParametersOutput
    {
        [JsonProperty("inactiveTime")]
        public uint InactiveTime { get; private set; }

        [JsonProperty("sleepTime")]
        public uint SleepTime { get; private set; }

        [JsonProperty("inactiveTimeUSBDisconnected")]
        public uint InactiveTimeUSBDisconnected { get; private set; }

        [JsonProperty("sleepTimeUSBDisconnected")]
        public uint SleepTimeUSBDisconnected { get; private set; }

        [JsonProperty("powerOffTimeUSBDisconnected")]
        public uint PowerOffTimeUSBDisconnected { get; private set; }

        [JsonProperty("ledPowerMaxLevel")]
        public byte LedPowerMaxLevel { get; private set; }

        [JsonProperty("ledPowerInactiveLevel")]
        public byte LedPowerInactiveLevel { get; private set; }

        [JsonProperty("brigthnessStep")]
        public byte BrigthnessStep { get; private set; }

        [JsonProperty("buttonLongPressDelay")]
        public ushort ButtonLongPressDelay { get; private set; }

        [JsonProperty("buttonRepeatLongPressDelay")]
        public ushort ButtonRepeatLongPressDelay { get; private set; }

        [JsonProperty("cleanRefreshPeriod")]
        public byte CleanRefreshPeriod { get; private set; }

        [JsonProperty("demoMode")]
        public bool DemoMode { get; private set; }

        [JsonProperty("lowBatteryBlinkOnDelayMs")]
        public ushort LowBatteryBlinkOnDelayMs { get; private set; }

        [JsonProperty("lowBatteryBlinkOffDelayMs")]
        public ushort LowBatteryBlinkOffDelayMs { get; private set; }

        [JsonProperty("lowBatteryLevelThresholdPercent")]
        public byte LowBatteryLevelThresholdPercent { get; private set; }

        [JsonProperty("bleBlinkOnDelayMs")]
        public ushort BleBlinkOnDelayMs { get; private set; }

        [JsonProperty("bleBlinkOffDelayMs")]
        public ushort BleBlinkOffDelayMs { get; private set; }

        [JsonProperty("highQualityModifier")]
        public bool HighQualityModifier { get; private set; }
        
        [JsonProperty("blackBackgroundCleanRefreshPeriod")]
        public byte BlackBackgroundCleanRefreshPeriod { get; private set; }

        [JsonProperty("brightnessStepList")]
        public List<byte> BrightnessStepList { get; private set; }

        public ParametersOutput(KeyboardParameters parameters)
        {
            InactiveTime = parameters.InactiveTime;
            SleepTime = parameters.SleepTime;
            InactiveTimeUSBDisconnected = parameters.InactiveTimeUSBDisconnected;
            SleepTimeUSBDisconnected = parameters.SleepTimeUSBDisconnected;
            PowerOffTimeUSBDisconnected = parameters.PowerOffTimeUSBDisconnected;
            LedPowerMaxLevel = parameters.LedPowerMaxLevel;
            LedPowerInactiveLevel = parameters.LedPowerInactiveLevel;
            BrigthnessStep = parameters.BrigthnessStep;
            ButtonLongPressDelay = parameters.ButtonLongPressDelay;
            ButtonRepeatLongPressDelay = parameters.ButtonRepeatLongPressDelay;
            CleanRefreshPeriod = parameters.CleanRefreshPeriod;
            DemoMode = parameters.DemoMode;
            LowBatteryBlinkOnDelayMs = parameters.LowBatteryBlinkOnDelayMs;
            LowBatteryBlinkOffDelayMs = parameters.LowBatteryBlinkOffDelayMs;
            LowBatteryLevelThresholdPercent = parameters.LowBatteryLevelThresholdPercent;
            BleBlinkOnDelayMs = parameters.BleBlinkOnDelayMs;
            BleBlinkOffDelayMs = parameters.BleBlinkOffDelayMs;
            HighQualityModifier = parameters.HighQualityPercent;
            BlackBackgroundCleanRefreshPeriod = parameters.BlackBackgroundCleanRefreshPeriod;
            BrightnessStepList = parameters.BrightnessStepList?.ToList() ?? new List<byte>();
        }
    }
}
