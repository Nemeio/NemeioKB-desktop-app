using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;
using System;
using System.Collections.Generic;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class CheckComponentsCommand : KeyboardCommand
    {
        private readonly byte _componentId;


        public CheckComponentsCommand(byte componentId)
            : base(CommandId.CheckComponents)
        {
            _componentId = componentId;

            Timeout = new TimeSpan(0, 0, 10);
        }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId.CheckComponents, new byte[] { _componentId }) };
    }
}
