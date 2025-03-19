using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;
using System;
using System.Collections.Generic;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class ExitFunctionalTestsCommand : KeyboardCommand
    {
        private readonly byte _validateState;

        public ExitFunctionalTestsCommand(byte validateState)
            : base(CommandId.ExitFunctionalTests)
        {
            _validateState = validateState;
            Timeout = new TimeSpan(0, 0, 5);
        }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId.ExitFunctionalTests, new byte[] { _validateState }) };

    }
}
