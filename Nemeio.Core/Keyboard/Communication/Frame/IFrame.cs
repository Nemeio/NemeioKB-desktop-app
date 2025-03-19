using Nemeio.Core.Device;

namespace Nemeio.Core.Keyboard.Communication.Frame
{
    public interface IFrame
    {
        CommandId CommandId { get; }
        byte[] Payload { get; }
        uint Crc { get; }
        byte[] Bytes { get; }
    }
}
