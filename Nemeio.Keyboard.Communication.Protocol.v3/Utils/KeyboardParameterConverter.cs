using System;
using System.Linq;
using Nemeio.Core;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v3.Utils
{
    public sealed class KeyboardParameterConverter : IKeyboardParameterParser
    {
        public const int PayloadSize = 43;

        public KeyboardParameters Parse(byte[] payload)
        {
            if (payload.Length < PayloadSize)
            {
                throw new ArgumentOutOfRangeException($"{nameof(payload)} parameter must be at least equals to {PayloadSize}");
            }

            var binaryReader = new NemeioBinaryReader(payload);

            //  Remove command
            binaryReader.ReadByte();

            //  Remove error state
            binaryReader.ReadByte();

            var keyboardParams = new KeyboardParameters();
            keyboardParams.InactiveTime = binaryReader.ReadUInt32();
            keyboardParams.SleepTime = binaryReader.ReadUInt32();
            keyboardParams.InactiveTimeUSBDisconnected = binaryReader.ReadUInt32();
            keyboardParams.SleepTimeUSBDisconnected = binaryReader.ReadUInt32();
            keyboardParams.PowerOffTimeUSBDisconnected = binaryReader.ReadUInt32();
            keyboardParams.LedPowerMaxLevel = binaryReader.ReadByte();
            keyboardParams.LedPowerInactiveLevel = binaryReader.ReadByte();
            keyboardParams.BrigthnessStep = binaryReader.ReadByte();
            keyboardParams.ButtonLongPressDelay = binaryReader.ReadUInt16();
            keyboardParams.ButtonRepeatLongPressDelay = binaryReader.ReadUInt16();
            keyboardParams.CleanRefreshPeriod = binaryReader.ReadByte();
            keyboardParams.DemoMode = binaryReader.ReadBoolean();
            keyboardParams.LowBatteryBlinkOnDelayMs = binaryReader.ReadUInt16();
            keyboardParams.LowBatteryBlinkOffDelayMs = binaryReader.ReadUInt16();
            keyboardParams.LowBatteryLevelThresholdPercent = binaryReader.ReadByte();
            keyboardParams.BleBlinkOnDelayMs = binaryReader.ReadUInt16();
            keyboardParams.BleBlinkOffDelayMs = binaryReader.ReadUInt16();
            keyboardParams.HighQualityPercent = binaryReader.ReadBoolean();
            keyboardParams.BlackBackgroundCleanRefreshPeriod = binaryReader.ReadByte();

            var brightnessStepListCount = binaryReader.ReadByte();

            //  Test if global size is valid
            var waitedSize = PayloadSize + brightnessStepListCount;
            if (payload.Length != waitedSize)
            {
                throw new ArgumentOutOfRangeException($"{nameof(payload)} parameter must be equals to {waitedSize}");
            }

            keyboardParams.BrightnessStepList = binaryReader.ReadByteList(brightnessStepListCount).ToList();

            return keyboardParams;
        }

        public byte[] ToByteArray(KeyboardParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            //  WARNING! BrightnessStepListCount and BrightnessStepList must always be sent on the end

            var content = new byte[0];
            content = content.Append(KeyboardProtocolHelpers.GetBytes(parameters.InactiveTime))
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.SleepTime))
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.InactiveTimeUSBDisconnected))
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.SleepTimeUSBDisconnected))
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.PowerOffTimeUSBDisconnected))
                .Append(new[] { parameters.LedPowerMaxLevel })
                .Append(new[] { parameters.LedPowerInactiveLevel })
                .Append(new[] { parameters.BrigthnessStep })
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.ButtonLongPressDelay))
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.ButtonRepeatLongPressDelay))
                .Append(new[] { parameters.CleanRefreshPeriod })
                .Append(BitConverter.GetBytes(parameters.DemoMode))
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.LowBatteryBlinkOnDelayMs))
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.LowBatteryBlinkOffDelayMs))
                .Append(new[] { parameters.LowBatteryLevelThresholdPercent })
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.BleBlinkOnDelayMs))
                .Append(KeyboardProtocolHelpers.GetBytes(parameters.BleBlinkOffDelayMs))
                .Append(BitConverter.GetBytes(parameters.HighQualityPercent))
                .Append(new[] { (byte)parameters.BlackBackgroundCleanRefreshPeriod })
                .Append(new[] { (byte)parameters.BrightnessStepList.Count });

            foreach (var brightnessStep in parameters.BrightnessStepList)
            {
                var value = new[] { brightnessStep };

                content = content.Append(value);
            }

            return content;
        }
    }
}
