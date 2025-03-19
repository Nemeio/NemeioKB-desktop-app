using System.Collections.Generic;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public abstract class EmptyPayloadCommand : KeyboardCommand
    {
        public EmptyPayloadCommand(CommandId commandId)
            : base(commandId) { }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId, new byte[0]) };
    }
}
