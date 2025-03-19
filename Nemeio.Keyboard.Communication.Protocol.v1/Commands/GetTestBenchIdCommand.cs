using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class GetTestBenchIdCommand : EmptyPayloadCommand
    {
        private readonly string _testId;

        public GetTestBenchIdCommand()
            : base(CommandId.GetTestBenchId)
        {


            Timeout = new TimeSpan(0, 0, 5);
        }
    }
}
