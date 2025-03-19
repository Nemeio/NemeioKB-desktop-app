using Nemeio.Core.Device;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class SerialNumberCommand : EmptyPayloadCommand
    {
        public SerialNumberCommand()
            : base(CommandId.SerialNumber) { }
    }
}
