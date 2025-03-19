using System;
using System.Collections.Generic;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Services.Layouts;
using Nemeio.Keyboard.Communication.Tools.Frames;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class ApplyConfigurationCommand : KeyboardCommand
    {
        private LayoutId _id;

        public ApplyConfigurationCommand(LayoutId id)
            : base(CommandId.ApplyConfig)
        {
            _id = id?? throw new ArgumentNullException(nameof(id));
        }

        public override IList<IFrame> ToFrames() => new List<IFrame>() { new SerialFrame(CommandId.ApplyConfig, _id.GetBytes()) };
    }
}
