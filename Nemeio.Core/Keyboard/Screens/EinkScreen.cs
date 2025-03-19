using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Images.Builders;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard.Map;

namespace Nemeio.Core.Keyboard.Screens
{
    public class EinkScreen : Screen
    {
        public EinkScreen(KeyboardMap map, IImageBuilder imageBuilder) 
            : base(map, imageBuilder, ScreenType.Eink) { }

        public override List<NemeioIndexKeystroke> TransformKeystrokes(IEnumerable<NemeioIndexKeystroke> keystrokes)
        {
            return keystrokes.ToList();
        }
    }
}
