using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class SetTestBenchIdCommand : KeyboardCommand
    {
        private readonly string _testId;

        public SetTestBenchIdCommand(string testId)
            : base(CommandId.SetTestBenchId)
        {
            _testId = testId;

            Timeout = new TimeSpan(0, 0, 5);
        }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId.SetTestBenchId, Encoding.ASCII.GetBytes(_testId)) };
    }
}
