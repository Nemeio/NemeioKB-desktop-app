using System.Collections.Generic;

namespace Nemeio.Core.Keyboard.Communication.Frame
{
    public interface IFrameParser
    {
        List<IFrame> FromByteArray(byte[] data);
    }
}
