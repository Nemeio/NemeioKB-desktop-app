using Nemeio.Core.Device;
using System;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class ClearScreenCommand : EmptyPayloadCommand
    {
        public ClearScreenCommand()
            : base(CommandId.ClearScreen) {
            Timeout = new TimeSpan(0, 0, 10);
        }
    }
}
