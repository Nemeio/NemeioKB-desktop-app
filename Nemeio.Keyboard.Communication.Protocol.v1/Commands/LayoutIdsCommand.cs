using System;
using Nemeio.Core.Device;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class LayoutIdsCommand : EmptyPayloadCommand
    {
        public override TimeSpan Timeout => new TimeSpan(0, 0, 10);

        public LayoutIdsCommand()
            : base(CommandId.LayoutIds) { }
    }
}
