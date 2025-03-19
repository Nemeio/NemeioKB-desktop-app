using System;
using System.Collections.Generic;
using Nemeio.Core.JsonModels;

namespace Nemeio.Core.Keyboard.Keys
{
    public class KeyPressedEventArgs
    {
        public IEnumerable<NemeioIndexKeystroke> Keystrokes { get; private set; }

        public KeyPressedEventArgs(IList<NemeioIndexKeystroke> keystrokes)
        {
            Keystrokes = keystrokes ?? throw new ArgumentNullException(nameof(keystrokes));
        }
    }
}
