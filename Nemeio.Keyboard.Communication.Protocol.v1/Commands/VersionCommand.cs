using System;
using Nemeio.Core.Device;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class VersionCommand : EmptyPayloadCommand
    {
        public override TimeSpan Timeout => new TimeSpan(0, 0, 5);

        public VersionCommand()
            : base(CommandId.Versions) { }
    }
}
