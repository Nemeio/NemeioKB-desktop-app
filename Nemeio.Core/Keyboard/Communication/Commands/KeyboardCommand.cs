using System;
using System.Collections.Generic;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Frame;

namespace Nemeio.Core.Keyboard.Communication.Commands
{
    public abstract class KeyboardCommand : IKeyboardCommand
    {
        public CommandId CommandId { get; }
        public virtual TimeSpan Timeout { get; protected set; } = new TimeSpan(0, 0, 1);
        public int FrameCount => ToFrames().Count;

        public KeyboardCommand(CommandId commandId)
        {
            CommandId = commandId;
        }

        public abstract IList<IFrame> ToFrames();
    }
}
