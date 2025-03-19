using System.Collections.Generic;
using Nemeio.Core.Keyboard.Communication.Frame;

namespace Nemeio.Keyboard.Communication.Tools.Frames
{
    public class FrameParser : IFrameParser
    {
        public List<IFrame> FromByteArray(byte[] data) => SerialFrame.Parse(data);
    }
}
