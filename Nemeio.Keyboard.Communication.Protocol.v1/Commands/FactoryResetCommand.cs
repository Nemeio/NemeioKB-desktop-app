using Nemeio.Core.Device;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class FactoryResetCommand : EmptyPayloadCommand
    {
        public FactoryResetCommand()
            : base(CommandId.FactoryReset) { }
    }
}
