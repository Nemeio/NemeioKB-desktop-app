using System;
using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Images;
using Nemeio.Core.Layouts.Images;

namespace Nemeio.Core.Services.Layouts
{
    public class LayoutRenderInfo
    {
        public LayoutImageType ImageType { get; private set; }
        public IEnumerable<Key> Keys { get; private set; }
        public Font MainFont { get; private set; }
        public bool IsDark { get; private set; }
        public ImageAdjustment Adjustment { get; private set; }

        public LayoutRenderInfo(LayoutImageType imageType, IEnumerable<Key> keys, Font mainFont, bool isDark, ImageAdjustment adjustment)
        {
            ImageType = imageType;
            Keys = keys ?? throw new ArgumentNullException(nameof(keys));
            MainFont = mainFont ?? throw new ArgumentNullException(nameof(mainFont));
            IsDark = isDark;
            Adjustment = adjustment;
        }
    }
}
