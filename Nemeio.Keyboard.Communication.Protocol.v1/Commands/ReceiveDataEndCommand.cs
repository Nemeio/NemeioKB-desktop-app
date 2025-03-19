using System.Collections.Generic;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.ReceiveData;
using Nemeio.Keyboard.Communication.Tools.Frames;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class ReceiveDataEndCommand : KeyboardCommand
    {
        private IReceiveData _type;

        public ReceiveDataEndCommand(IReceiveData type)
            : base(Core.Device.CommandId.ReceiveData) => _type = type;

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId, new[] { _type.Value, (byte)ReceiveDataCommandType.Stop }) };
    }
}
