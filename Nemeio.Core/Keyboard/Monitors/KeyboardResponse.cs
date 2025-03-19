using System;
using Nemeio.Core.Keyboard.Communication.Frame;

namespace Nemeio.Core.Keyboard.Monitors
{
    public class KeyboardResponse
    {
        public IFrame Frame { get; set; }
        public Exception Exception { get; set; }
    }
}
