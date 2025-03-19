using Nemeio.Core.Device;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class BatteryCommand : EmptyPayloadCommand
    {
        public BatteryCommand()
            : base(CommandId.Battery) { }
    }
}
