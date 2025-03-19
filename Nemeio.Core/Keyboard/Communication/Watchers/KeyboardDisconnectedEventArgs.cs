using System;

namespace Nemeio.Core.Keyboard.Communication.Watchers
{
    public class KeyboardDisconnectedEventArgs
    {
        public string Identifier { get; private set; }

        public KeyboardDisconnectedEventArgs(Keyboard keyboard)
            : this(keyboard.Identifier) { }

        public KeyboardDisconnectedEventArgs(string identifier)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        }
    }
}
