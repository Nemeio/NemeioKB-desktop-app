namespace Nemeio.Core.Systems.Sessions
{
    public sealed class SessionStateChangedEventArgs
    {
        public SessionState State { get; private set; }

        public SessionStateChangedEventArgs(SessionState state)
        {
            State = state;
        }
    }
}
