using System;
using Nemeio.Api.Dto.In.Events;
using Nemeio.Api.Keyboard.Parameters.Base;
using Nemeio.Api.Keyboard.Parameters.Screen;
using Nemeio.Core.Keyboard.Parameters;

namespace Nemeio.Api.Keyboard.Parameters
{
    internal class KeyboardParameterFactory
    {
        internal IParameter Create(KeyboardParameters parameters, EventType type)
        {
            switch (type)
            {
                case EventType.InactiveTime:
                    return new InactiveTimeParameter(parameters);
                case EventType.SleepTime:
                    return new SleepTimeParameter(parameters);
                case EventType.InactiveTimeUSBDisconnected:
                    return new InactiveTimeUsbDisconnectedParameter(parameters);
                case EventType.SleepTimeUSBDisconnected:
                    return new SleepTimeUsbDisconnectedParameter(parameters);
                case EventType.PowerOffTimeUSBDisconnected:
                    return new PowerOffTimeUsbDisconnectedParameter(parameters);
                case EventType.LedPowerMaxLevel:
                    return new LedPowerMaxLevelParameter(parameters);
                case EventType.LedPowerInactiveLevel:
                    return new LedPowerInactiveLevelParameter(parameters);
                case EventType.BrightnessStep:
                    return new BrightnessStepParameter(parameters);
                case EventType.ButtonLongPressDelay:
                    return new ButtonLongPressDelayParameter(parameters);
                case EventType.ButtonRepeatLongPressDelay:
                    return new ButtonRepeatLongPressDelayParameter(parameters);
                case EventType.CleanRefreshPeriod:
                    return new CleanRefreshPeriodParameter(parameters);
                case EventType.DemoMode:
                    return new DemoModeParameter(parameters);
                case EventType.LowBatteryBlinkOnDelay:
                    return new LowBatteryBlinkOnDelayParameter(parameters);
                case EventType.LowBatteryBlinkOffDelay:
                    return new LowBatteryBlinkOffDelayParameter(parameters);
                case EventType.LowBatteryLevelThreshold:
                    return new LowBatteryLevelThresholdParameter(parameters);
                case EventType.BleBlinkOnDelay:
                    return new BleBlinkOnDelayParameter(parameters);
                case EventType.BleBlinkOffDelay:
                    return new BleBlinkOffDelayParameter(parameters);
                case EventType.HighQualityModifier:
                    return new HighQualityModifierParameter(parameters);
                case EventType.BrightnessStepList:
                    return new BrightnessStepListParameter(parameters);
                case EventType.BlackBackgroundCleanRefreshPeriod:
                    return new BlackBackgroundCleanRefreshPeriodParameter(parameters);
                default:
                    throw new ArgumentNullException(nameof(type));
            }
        }
    }
}
