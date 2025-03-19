using System.Collections.Generic;
using System.Linq;
using Nemeio.Api.Dto.In.Events;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Api.Keyboard.Parameters.Dto.Out
{
    public class KeyboardParametersOutDto
    {
        [JsonProperty("inactiveTime")]
        public KeyboardParameterOutDto<uint> InactiveTime { get; private set; }

        [JsonProperty("sleepTime")]
        public KeyboardParameterOutDto<uint> SleepTime { get; private set; }

        [JsonProperty("inactiveTimeUSBDisconnected")]
        public KeyboardParameterOutDto<uint> InactiveTimeUSBDisconnected { get; private set; }

        [JsonProperty("sleepTimeUSBDisconnected")]
        public KeyboardParameterOutDto<uint> SleepTimeUSBDisconnected { get; private set; }

        [JsonProperty("powerOffTimeUSBDisconnected")]
        public KeyboardParameterOutDto<uint> PowerOffTimeUSBDisconnected { get; private set; }

        [JsonProperty("ledPowerMaxLevel")]
        public KeyboardParameterOutDto<byte> LedPowerMaxLevel { get; private set; }

        [JsonProperty("ledPowerInactiveLevel")]
        public KeyboardParameterOutDto<byte> LedPowerInactiveLevel { get; private set; }

        [JsonProperty("brigthnessStep")]
        public KeyboardParameterOutDto<byte> BrigthnessStep { get; private set; }

        [JsonProperty("buttonLongPressDelay")]
        public KeyboardParameterOutDto<ushort> ButtonLongPressDelay { get; private set; }

        [JsonProperty("buttonRepeatLongPressDelay")]
        public KeyboardParameterOutDto<ushort> ButtonRepeatLongPressDelay { get; private set; }

        [JsonProperty("cleanRefreshPeriod")]
        public KeyboardParameterOutDto<byte> CleanRefreshPeriod { get; private set; }

        [JsonProperty("demoMode")]
        public KeyboardParameterOutDto<bool> DemoMode { get; private set; }

        [JsonProperty("lowBatteryBlinkOnDelayMs")]
        public KeyboardParameterOutDto<ushort> LowBatteryBlinkOnDelayMs { get; private set; }

        [JsonProperty("lowBatteryBlinkOffDelayMs")]
        public KeyboardParameterOutDto<ushort> LowBatteryBlinkOffDelayMs { get; private set; }

        [JsonProperty("lowBatteryLevelThresholdPercent")]
        public KeyboardParameterOutDto<byte> LowBatteryLevelThresholdPercent { get; private set; }

        [JsonProperty("bleBlinkOnDelayMs")]
        public KeyboardParameterOutDto<ushort> BleBlinkOnDelayMs { get; private set; }

        [JsonProperty("bleBlinkOffDelayMs")]
        public KeyboardParameterOutDto<ushort> BleBlinkOffDelayMs { get; private set; }

        [JsonProperty("highQualityModifier")]
        public KeyboardParameterOutDto<bool> HighQualityModifier { get; private set; }

        [JsonProperty("brightnessStepList")]
        public KeyboardParameterOutDto<List<byte>> BrightnessStepList { get; private set; }

        [JsonProperty("blackBackgroundCleanRefreshPeriod")]
        public KeyboardParameterOutDto<byte> BlackBackgroundCleanRefreshPeriod { get; private set; }

        public static KeyboardParametersOutDto FromModel(KeyboardParameters parameters)
        {
            return new KeyboardParametersOutDto()
            {
                InactiveTime = new KeyboardParameterOutDto<uint>(EventType.InactiveTime, parameters.InactiveTime),
                SleepTime = new KeyboardParameterOutDto<uint>(EventType.SleepTime, parameters.SleepTime),
                InactiveTimeUSBDisconnected = new KeyboardParameterOutDto<uint>(EventType.InactiveTimeUSBDisconnected, parameters.InactiveTimeUSBDisconnected),
                SleepTimeUSBDisconnected = new KeyboardParameterOutDto<uint>(EventType.SleepTimeUSBDisconnected, parameters.SleepTimeUSBDisconnected),
                PowerOffTimeUSBDisconnected = new KeyboardParameterOutDto<uint>(EventType.PowerOffTimeUSBDisconnected, parameters.PowerOffTimeUSBDisconnected),
                LedPowerMaxLevel = new KeyboardParameterOutDto<byte>(EventType.LedPowerMaxLevel, parameters.LedPowerMaxLevel),
                LedPowerInactiveLevel = new KeyboardParameterOutDto<byte>(EventType.LedPowerInactiveLevel, parameters.LedPowerInactiveLevel),
                BrigthnessStep = new KeyboardParameterOutDto<byte>(EventType.BrightnessStep, parameters.BrigthnessStep),
                ButtonLongPressDelay = new KeyboardParameterOutDto<ushort>(EventType.ButtonLongPressDelay, parameters.ButtonLongPressDelay),
                ButtonRepeatLongPressDelay = new KeyboardParameterOutDto<ushort>(EventType.ButtonRepeatLongPressDelay, parameters.ButtonRepeatLongPressDelay),
                CleanRefreshPeriod = new KeyboardParameterOutDto<byte>(EventType.CleanRefreshPeriod, parameters.CleanRefreshPeriod),
                DemoMode = new KeyboardParameterOutDto<bool>(EventType.DemoMode, parameters.DemoMode),
                LowBatteryBlinkOnDelayMs = new KeyboardParameterOutDto<ushort>(EventType.LowBatteryBlinkOnDelay, parameters.LowBatteryBlinkOnDelayMs),
                LowBatteryBlinkOffDelayMs = new KeyboardParameterOutDto<ushort>(EventType.LowBatteryBlinkOffDelay, parameters.LowBatteryBlinkOffDelayMs),
                LowBatteryLevelThresholdPercent = new KeyboardParameterOutDto<byte>(EventType.LowBatteryLevelThreshold, parameters.LowBatteryLevelThresholdPercent),
                BleBlinkOnDelayMs = new KeyboardParameterOutDto<ushort>(EventType.BleBlinkOnDelay, parameters.BleBlinkOnDelayMs),
                BleBlinkOffDelayMs = new KeyboardParameterOutDto<ushort>(EventType.BleBlinkOffDelay, parameters.BleBlinkOffDelayMs),
                HighQualityModifier = new KeyboardParameterOutDto<bool>(EventType.HighQualityModifier, parameters.HighQualityPercent),
                BrightnessStepList = new KeyboardParameterOutDto<List<byte>>(EventType.BrightnessStepList, parameters.BrightnessStepList.ToList()),
                BlackBackgroundCleanRefreshPeriod = new KeyboardParameterOutDto<byte>(EventType.BlackBackgroundCleanRefreshPeriod, parameters.BlackBackgroundCleanRefreshPeriod)
            };
        }
    }
}
