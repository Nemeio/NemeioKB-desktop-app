using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;
using System;
using System.Collections.Generic;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class DisplayCheckerBoardCommand : KeyboardCommand
    {
        private readonly byte _firstColor;
        public DisplayCheckerBoardCommand(byte firstColor)
            : base(CommandId.DisplayCheckerBoard)
        {
            _firstColor = firstColor;
            Timeout = new TimeSpan(0, 0, 5);
        }
        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId.DisplayCheckerBoard, new byte[] { _firstColor }) };
    }
}
