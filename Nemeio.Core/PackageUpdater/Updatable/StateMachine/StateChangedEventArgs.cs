using System;
using Nemeio.Core.PackageUpdater.Updatable.States;

namespace Nemeio.Core.PackageUpdater.Updatable.StateMachine
{
    public sealed class StateChangedEventArgs
    {
        public UpdateState PreviousState { get; private set; }
        public UpdateState State { get; private set; }

        public StateChangedEventArgs(UpdateState previous, UpdateState state)
        {
            PreviousState = previous ?? throw new ArgumentNullException(nameof(previous));
            State = state ?? throw new ArgumentNullException(nameof(state));
        }
    }
}
