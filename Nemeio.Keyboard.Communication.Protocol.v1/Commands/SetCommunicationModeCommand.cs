using System.Collections.Generic;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Keyboard.Communication.Tools.Frames;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class SetCommunicationModeCommand : KeyboardCommand
    {
        private readonly KeyboardCommunicationMode _communicationMode;

        public SetCommunicationModeCommand(KeyboardCommunicationMode communicationMode)
            : base (CommandId.SetCommMode)
        {
            _communicationMode = communicationMode;
        }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId, new[] { (byte)_communicationMode }) };
    }
}
