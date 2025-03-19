using System;
using Nemeio.Core.Keyboard.Communication.Watchers;
using CommKeyboard = Nemeio.Core.Keyboard.Keyboard;

namespace Nemeio.Core.Keyboard.Communication
{
    public interface IKeyboardProvider
    {
        event EventHandler OnKeyboardConnected;
        event EventHandler<KeyboardDisconnectedEventArgs> OnKeyboardDisconnected;

        CommKeyboard GetKeyboard(Func<CommKeyboard, bool> filter);
    }
}
