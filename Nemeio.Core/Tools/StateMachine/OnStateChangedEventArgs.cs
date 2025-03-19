namespace Nemeio.Core.Tools.StateMachine
{
    public sealed class OnStateChangedEventArgs<TState>
    {
        public TState PreviousState { get; private set; }
        public TState State { get; private set; }

        public OnStateChangedEventArgs(TState previousState, TState state)
        {
            PreviousState = previousState;
            State = state;
        }
    }
}
