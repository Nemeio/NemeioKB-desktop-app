using System;
using System.Collections.Generic;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Frame;

namespace Nemeio.Core.Keyboard.Communication.Commands
{
    public interface IKeyboardCommand
    {
        CommandId CommandId { get; }
        TimeSpan Timeout { get; }
        int FrameCount { get; }
        IList<IFrame> ToFrames();
    }
}
