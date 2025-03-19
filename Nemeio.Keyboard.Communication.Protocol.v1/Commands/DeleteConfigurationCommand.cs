using System;
using System.Collections.Generic;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Services.Layouts;
using Nemeio.Keyboard.Communication.Tools.Frames;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class DeleteConfigurationCommand : KeyboardCommand
    {
        private LayoutId _id;

        public DeleteConfigurationCommand(LayoutId id)
            : base(CommandId.DeleteConfig)
        {
            _id= id?? throw new ArgumentNullException(nameof(id));

            Timeout = new TimeSpan(0, 0, 10);
        }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId.DeleteConfig, _id.GetBytes()) };
    }
}
