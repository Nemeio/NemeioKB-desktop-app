using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.Parameters.Set
{
    internal sealed class ParametersInput
    {
        [JsonProperty("inactiveTime")]
        public uint? InactiveTime { get; private set; }

        [JsonProperty("sleepTime")]
        public uint? SleepTime { get; private set; }

        [JsonProperty("inactiveTimeUSBDisconnected")]
        public uint? InactiveTimeUSBDisconnected { get; private set; }

        [JsonProperty("sleepTimeUSBDisconnected")]
        public uint? SleepTimeUSBDisconnected { get; private set; }

        [JsonProperty("powerOffTimeUSBDisconnected")]
        public uint? PowerOffTimeUSBDisconnected { get; private set; }

        [JsonProperty("ledPowerMaxLevel")]
        public byte? LedPowerMaxLevel { get; private set; }

        [JsonProperty("ledPowerInactiveLevel")]
        public byte? LedPowerInactiveLevel { get; private set; }

        [JsonProperty("brigthnessStep")]
        public byte? BrigthnessStep { get; private set; }

        [JsonProperty("buttonLongPressDelay")]
        public ushort? ButtonLongPressDelay { get; private set; }

        [JsonProperty("buttonRepeatLongPressDelay")]
        public ushort? ButtonRepeatLongPressDelay { get; private set; }

        [JsonProperty("cleanRefreshPeriod")]
        public byte? CleanRefreshPeriod { get; private set; }

        [JsonProperty("demoMode")]
        public bool? DemoMode { get; private set; }

        [JsonProperty("lowBatteryBlinkOnDelayMs")]
        public ushort? LowBatteryBlinkOnDelayMs { get; private set; }

        [JsonProperty("lowBatteryBlinkOffDelayMs")]
        public ushort? LowBatteryBlinkOffDelayMs { get; private set; }

        [JsonProperty("lowBatteryLevelThresholdPercent")]
        public byte? LowBatteryLevelThresholdPercent { get; private set; }

        [JsonProperty("bleBlinkOnDelayMs")]
        public ushort? BleBlinkOnDelayMs { get; private set; }

        [JsonProperty("bleBlinkOffDelayMs")]
        public ushort? BleBlinkOffDelayMs { get; private set; }

        [JsonProperty("highQualityModifier")]
        public bool? HighQualityModifier { get; private set; }
        
        [JsonProperty("blackBackgroundCleanRefreshPeriod")]
        public byte? BlackBackgroundCleanRefreshPeriod { get; private set; }

        [JsonProperty("brightnessStepList")]
        public List<byte> BrightnessStepList { get; private set; }

        public void Patch(KeyboardParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (InactiveTime.HasValue)
            {
                parameters.InactiveTime = InactiveTime.Value;
            }

            if (SleepTime.HasValue)
            {
                parameters.SleepTime = SleepTime.Value;
            }

            if (InactiveTimeUSBDisconnected.HasValue)
            {
                parameters.InactiveTimeUSBDisconnected = InactiveTimeUSBDisconnected.Value;
            }

            if (SleepTimeUSBDisconnected.HasValue)
            {
                parameters.SleepTimeUSBDisconnected = SleepTimeUSBDisconnected.Value;
            }

            if (PowerOffTimeUSBDisconnected.HasValue)
            {
                parameters.PowerOffTimeUSBDisconnected = PowerOffTimeUSBDisconnected.Value;
            }

            if (LedPowerMaxLevel.HasValue)
            {
                parameters.LedPowerMaxLevel = LedPowerMaxLevel.Value;
            }

            if (LedPowerInactiveLevel.HasValue)
            {
                parameters.LedPowerInactiveLevel = LedPowerInactiveLevel.Value;
            }

            if (BrigthnessStep.HasValue)
            {
                parameters.BrigthnessStep = BrigthnessStep.Value;
            }

            if (ButtonLongPressDelay.HasValue)
            {
                parameters.ButtonLongPressDelay = ButtonLongPressDelay.Value;
            }

            if (ButtonRepeatLongPressDelay.HasValue)
            {
                parameters.ButtonRepeatLongPressDelay = ButtonRepeatLongPressDelay.Value;
            }

            if (CleanRefreshPeriod.HasValue)
            {
                parameters.CleanRefreshPeriod = CleanRefreshPeriod.Value;
            }

            if (DemoMode.HasValue)
            {
                parameters.DemoMode = DemoMode.Value;
            }

            if (LedPowerInactiveLevel.HasValue)
            {
                parameters.LedPowerInactiveLevel = LedPowerInactiveLevel.Value;
            }

            if (LowBatteryBlinkOnDelayMs.HasValue)
            {
                parameters.LowBatteryBlinkOnDelayMs = LowBatteryBlinkOnDelayMs.Value;
            }

            if (LowBatteryBlinkOffDelayMs.HasValue)
            {
                parameters.LowBatteryBlinkOffDelayMs = LowBatteryBlinkOffDelayMs.Value;
            }

            if (LowBatteryLevelThresholdPercent.HasValue)
            {
                parameters.LowBatteryLevelThresholdPercent = LowBatteryLevelThresholdPercent.Value;
            }

            if (BleBlinkOnDelayMs.HasValue)
            {
                parameters.BleBlinkOnDelayMs = BleBlinkOnDelayMs.Value;
            }

            if (BleBlinkOffDelayMs.HasValue)
            {
                parameters.BleBlinkOffDelayMs = BleBlinkOffDelayMs.Value;
            }

            if (HighQualityModifier.HasValue)
            {
                parameters.HighQualityPercent = HighQualityModifier.Value;
            }
            
            if (BlackBackgroundCleanRefreshPeriod.HasValue)
            {
                parameters.BlackBackgroundCleanRefreshPeriod = BlackBackgroundCleanRefreshPeriod.Value;
            }

            if (BrightnessStepList != null)
            {
                parameters.BrightnessStepList = BrightnessStepList;
            }
        }
    }
}
