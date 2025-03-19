using System;

namespace Nemeio.Core.Keyboard.State
{
    public interface IStateHolder
    {
        NemeioState State { get; }

        event EventHandler<StateChangedEventArgs> OnStateChanged;
    }
}
