using System.Collections.Generic;
using Nemeio.Core.Images;
using Nemeio.Core.Images.Builders;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard.Map;

namespace Nemeio.Core.Keyboard.Screens
{
    public interface IScreen
    {
        KeyboardMap Map { get; }
        IImageBuilder Builder { get; set; }
        ScreenType Type { get; }
        ImageFormat Format { get; }

        List<NemeioIndexKeystroke> TransformKeystrokes(IEnumerable<NemeioIndexKeystroke> keystrokes);
    }
}
