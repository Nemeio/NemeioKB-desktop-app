using System;
using Nemeio.Core;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Utils
{
    public class KeyboardParameterConverter : IKeyboardParameterParser
    {
        public const int PayloadSize = 39;

        public KeyboardParameters Parse(byte[] payload)
        {
            payload = payload.SubArray(2, PayloadSize);

            if (payload.Length != PayloadSize)
            {
                throw new ArgumentOutOfRangeException($"'payload' parameter must be equal to {PayloadSize}");
            }

            var binaryReader = new NemeioBinaryReader(payload);

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

            return keyboardParams;
        }

        public byte[] ToByteArray(KeyboardParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

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
                .Append(BitConverter.GetBytes(parameters.HighQualityPercent));

            return content;
        }
    }
}
