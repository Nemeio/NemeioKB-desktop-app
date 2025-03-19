using System;

namespace Nemeio.Core.Keyboard.Connection
{
    public sealed class KeyboardInitializationFailedEventArgs : EventArgs
    {
        public string Identifier { get; private set; }

        public KeyboardInitializationFailedEventArgs(string identifier)
        {
            Identifier = identifier;
        }
    }
}
