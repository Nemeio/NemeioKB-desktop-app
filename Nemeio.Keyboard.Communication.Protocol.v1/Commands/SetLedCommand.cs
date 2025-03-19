using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;
using System;
using System.Collections.Generic;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class SetLedCommand : KeyboardCommand
    {
        private readonly byte _ledId;
        private readonly byte _ledState;

        public SetLedCommand(byte ledId, byte ledState)
            : base(CommandId.SetLed)
        {
            _ledId = ledId;
            _ledState = ledState;

            Timeout = new TimeSpan(0, 0, 5);
        }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId.SetLed, new byte[] { _ledId, _ledState }) };
    }
}
