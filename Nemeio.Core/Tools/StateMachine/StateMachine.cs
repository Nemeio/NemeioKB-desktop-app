using System;

namespace Nemeio.Core.Tools.StateMachine
{
    public abstract class StateMachine<TState> : IStateMachine<TState>
    {
        private static object _lockState = new object();

        private TState _state;

        public event EventHandler<OnStateChangedEventArgs<TState>> OnStateChanged;

        public TState State
        {
            get => _state;
            protected set
            {
                lock (_lockState)
                {
                    var previousState = _state;

                    _state = value;

                    RaiseOnStateChanged(previousState, value);
                }
            }
        }

        private void RaiseOnStateChanged(TState previousState, TState state)
        {
            var eventArgs = new OnStateChangedEventArgs<TState>(previousState, state);

            OnStateChanged?.Invoke(this, eventArgs);
        }
    }
}
