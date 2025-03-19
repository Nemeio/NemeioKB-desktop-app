using System;

namespace Nemeio.Core.Keyboard.Keys
{
    public interface IKeyPressedMonitor
    {
        event EventHandler<KeyPressedEventArgs> OnKeyPressed;
    }
}
