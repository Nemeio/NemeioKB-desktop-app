using System;
using System.Collections.Generic;
using Nemeio.Core.Images;
using Nemeio.Core.Images.Builders;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard.Map;

namespace Nemeio.Core.Keyboard.Screens
{
    public abstract class Screen : IScreen
    {
        public KeyboardMap Map { get; private set; }
        public IImageBuilder Builder { get; set; }
        public ScreenType Type { get; private set; }
        public ImageFormat Format => Builder.Format;

        public Screen(KeyboardMap map, IImageBuilder builder, ScreenType type)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
            Builder = builder ?? throw new ArgumentNullException(nameof(builder));
            Type = type;
        }

        public abstract List<NemeioIndexKeystroke> TransformKeystrokes(IEnumerable<NemeioIndexKeystroke> keystrokes);
    }
}
