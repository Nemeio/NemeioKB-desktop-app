using Nemeio.Core.Device;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class KeepAliveCommand : EmptyPayloadCommand
    {
        public KeepAliveCommand()
            : base(CommandId.KeepAlive) { }
    }
}
