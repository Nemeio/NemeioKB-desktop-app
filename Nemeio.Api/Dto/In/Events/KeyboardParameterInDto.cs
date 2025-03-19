using Nemeio.Api.Converters;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Events
{
    public enum EventType
    {
        InactiveTime = 0,
        SleepTime = 1,
        InactiveTimeUSBDisconnected = 2,
        SleepTimeUSBDisconnected = 3,
        PowerOffTimeUSBDisconnected = 4,
        LedPowerMaxLevel = 5,
        LedPowerInactiveLevel = 6,
        BrightnessStep = 7,
        ButtonLongPressDelay = 8,
        ButtonRepeatLongPressDelay = 9,
        CleanRefreshPeriod = 10,
        DemoMode = 13,
        LowBatteryBlinkOnDelay = 14,
        LowBatteryBlinkOffDelay = 15,
        LowBatteryLevelThreshold = 16,
        BleBlinkOnDelay = 17,
        BleBlinkOffDelay = 18,
        HighQualityModifier = 19,
        BrightnessStepList = 21,
        BlackBackgroundCleanRefreshPeriod = 22
    }

    /// <summary>
    /// Parameter information
    /// </summary>
    public class KeyboardParameterInDto
    {
        /// <summary>
        /// Parameter's type
        /// </summary>
        [JsonProperty("type", Required = Required.Always)]
        [JsonConverter(typeof(KeyboardParameterTypeConverter))]
        public EventType Type { get; set; }

        /// <summary>
        /// New parameter's value
        /// </summary>
        [JsonProperty("data", Required = Required.Always)]
        public string Data { get; set; }
    }
}
