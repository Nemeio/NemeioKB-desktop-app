using System;

namespace Nemeio.Core.Tools.StateMachine
{
    public interface IStateMachine<TState>
    {
        event EventHandler<OnStateChangedEventArgs<TState>> OnStateChanged;

        TState State { get; }
    }
}
