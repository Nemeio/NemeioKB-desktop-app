using System;
using System.Collections.Generic;

namespace Nemeio.Core.Keyboard.Communication.Watchers
{
    public interface IKeyboardWatcher
    {
        IList<Keyboard> Keyboards { get; }
        event EventHandler OnKeyboardConnected;
        event EventHandler<KeyboardDisconnectedEventArgs> OnKeyboardDisconnected;
    }
}
