using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class CheckIfBatteryPresentCommand : EmptyPayloadCommand
    {
        public CheckIfBatteryPresentCommand()
            : base(CommandId.CheckIfBatteryPresent)
        {
            Timeout = new TimeSpan(0, 0, 5);
        }
    }
}
