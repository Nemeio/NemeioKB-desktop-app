using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Services.Layouts;
using Nemeio.Keyboard.Communication.Tools.Frames;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class SendConfigurationCommand : SendDataCommand
    {
        private readonly ILayout _layout;
        private readonly bool _isFactoryLayout;

        public override TimeSpan Timeout => new TimeSpan(0, 30, 0);

        public SendConfigurationCommand(ILayout layout, bool isFactoryLayout = false)
        {
            _layout = layout ?? throw new ArgumentNullException(nameof(layout));

            Timeout = new TimeSpan(0, 0, 10);
            _isFactoryLayout = isFactoryLayout;
        }

        public override IList<IFrame> ToFrames()
        {
            var dto = new LayoutInfoDto(_layout);

            var configurationPayloads = CreateSendDataPayloads(_isFactoryLayout ? FrameDataType.DefaultConfiguration : FrameDataType.Configuration, dto.GetBytes());
            var wallpaperPayloads = CreateSendDataPayloads(_isFactoryLayout ? FrameDataType.DefaultWallpaper : FrameDataType.Wallpaper, _layout.Image);

            var frames = new List<IFrame>()
                .AddChainable(configurationPayloads)
                .AddChainable(wallpaperPayloads);

            return frames;
        }
    }
}
