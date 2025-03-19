using System;
using System.Collections.Generic;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Keyboard.Communication.Tools.Frames;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class SetParameterCommand : KeyboardCommand
    {
        private readonly KeyboardParameters _parameters;
        private readonly IKeyboardParameterParser _parser;

        public SetParameterCommand(KeyboardParameters parameters, IKeyboardParameterParser parser)
            : base(CommandId.KeyboardParameters)
        {
            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId, new[] { (byte)ParameterCommand.SetParameters }.Append(_parser.ToByteArray(_parameters))) };
    }
}
