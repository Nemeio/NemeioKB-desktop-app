using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;
using System;
using System.Collections.Generic;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class SetFrontLightPowerCommand : KeyboardCommand
    {
        private readonly byte _power;

        public SetFrontLightPowerCommand(byte power)
            : base(CommandId.SetLed)
        {
            _power = power;

            Timeout = new TimeSpan(0, 0, 5);
        }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId.SetFrontLightPower, new byte[] { _power}) };
    }
}
