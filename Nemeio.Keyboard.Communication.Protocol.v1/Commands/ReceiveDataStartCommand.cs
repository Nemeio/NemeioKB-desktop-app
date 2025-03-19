using System.Collections.Generic;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.ReceiveData;
using Nemeio.Keyboard.Communication.Tools.Frames;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class ReceiveDataStartCommand : KeyboardCommand
    {
        private IReceiveData _type;

        public ReceiveDataStartCommand(IReceiveData type)
            : base(CommandId.ReceiveData) => _type = type;

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId, new[] { _type.Value, (byte)ReceiveDataCommandType.Start }) };
    }
}
