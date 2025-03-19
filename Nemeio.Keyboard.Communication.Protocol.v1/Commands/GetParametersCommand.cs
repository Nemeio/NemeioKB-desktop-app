using System.Collections.Generic;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Keyboard.Communication.Tools.Frames;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class GetParametersCommand : KeyboardCommand
    {
        public GetParametersCommand() 
            : base(CommandId.KeyboardParameters) { }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId, new[] { (byte)ParameterCommand.GetParameters }) };
    }
}
