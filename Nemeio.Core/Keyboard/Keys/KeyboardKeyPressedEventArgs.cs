using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Keyboard.Keys
{
    public sealed class KeyboardKeyPressedEventArgs
    {
        public LayoutId SelectedLayout { get; private set; }
        public IList<NemeioIndexKeystroke> Keystrokes { get; private set; }

        public KeyboardKeyPressedEventArgs(LayoutId selectedLayout, IEnumerable<NemeioIndexKeystroke> keystrokes)
        {
            SelectedLayout = selectedLayout ?? throw new ArgumentNullException(nameof(selectedLayout));
            Keystrokes = keystrokes.ToList() ?? throw new ArgumentNullException(nameof(keystrokes));
        }
    }
}
