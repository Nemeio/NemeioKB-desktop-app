using System;
using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Images
{
    public sealed class ImageRequest
    {
        public LayoutInfo Info { get; private set; }
        public LayoutImageInfo ImageInfo { get; private set; }
        public IEnumerable<Key> Keys { get; private set; }
        public IScreen Screen { get; private set; }
        public ImageAdjustment Adjustment { get; private set; }

        public ImageRequest(LayoutInfo info, LayoutImageInfo imageInfo, IEnumerable<Key> keys, IScreen screen, ImageAdjustment adjustment)
        {
            Info = info ?? throw new ArgumentNullException(nameof(info));
            ImageInfo = imageInfo ?? throw new ArgumentNullException(nameof(imageInfo));
            Keys = keys ?? throw new ArgumentNullException(nameof(keys));
            Screen = screen;
            Adjustment = adjustment;
        }
    }
}
