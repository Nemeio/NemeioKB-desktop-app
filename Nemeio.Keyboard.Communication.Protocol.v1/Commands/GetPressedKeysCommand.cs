using Nemeio.Core.Device;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class GetPressedKeysCommand : EmptyPayloadCommand
    {
        public GetPressedKeysCommand()
            : base(CommandId.GetPressedKeys) { }
    }
}
