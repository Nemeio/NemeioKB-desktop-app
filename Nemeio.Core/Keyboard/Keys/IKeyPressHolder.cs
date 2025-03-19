using System;

namespace Nemeio.Core.Keyboard.Keys
{
    public interface IKeyPressHolder
    {
        event EventHandler<KeyboardKeyPressedEventArgs> OnKeyPressed;
    }
}
