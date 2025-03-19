using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Images.Builders;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard.Map;

namespace Nemeio.Core.Keyboard.Screens
{
    public class HolitechScreen : Screen
    {
        public HolitechScreen(KeyboardMap map, IImageBuilder imageBuilder) 
            : base(map, imageBuilder, ScreenType.Holitech) { }

        public override List<NemeioIndexKeystroke> TransformKeystrokes(IEnumerable<NemeioIndexKeystroke> keystrokes)
        {
            //  Nothing to do here
            return keystrokes.ToList();
        }
    }
}
