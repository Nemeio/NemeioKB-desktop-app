using System;
using System.Collections.Generic;

namespace Nemeio.Core.Keyboard.Map
{
    public class KeyboardMap
    {
        public Size ScreenSize { get; private set; }
        public IList<KeyboardButton> Buttons { get; private set; }
        public bool FlipHorizontal { get; private set; }

        public KeyboardMap(float width, float height, IList<KeyboardButton> buttons, bool flipVertical)
        {
            ScreenSize = new Size(width, height);
            Buttons = buttons ?? throw new ArgumentNullException(nameof(buttons));
            FlipHorizontal = flipVertical;
        }
    }
}
