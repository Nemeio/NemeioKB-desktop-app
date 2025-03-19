using System;
using Nemeio.Core.Keyboard.Nemeios;

namespace Nemeio.Core.Keyboard.Connection
{
    public class KeyboardStatusChangedEventArgs : EventArgs
    {
        public IKeyboard Keyboard { get; private set; }

        public KeyboardStatusChangedEventArgs(IKeyboard keyboard)
        {
            Keyboard = keyboard;
        }
    }
}
