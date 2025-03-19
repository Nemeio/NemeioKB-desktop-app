using Nemeio.Core.Device;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class GetBatteryElectricalStatusCommand : EmptyPayloadCommand
    {
        public GetBatteryElectricalStatusCommand()
            : base(CommandId.GetBatteryElectricalStatus) { }
    }
}
