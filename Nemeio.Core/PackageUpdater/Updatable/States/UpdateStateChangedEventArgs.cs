using System;

namespace Nemeio.Core.PackageUpdater.Updatable.States
{
    public class UpdateStateChangedEventArgs
    {
        public UpdateState PreviousState { get; private set; }
        public UpdateState State { get; private set; }

        public UpdateStateChangedEventArgs(UpdateState previous, UpdateState state)
        {
            PreviousState = previous;
            State = state ?? throw new ArgumentNullException(nameof(state));
        }
    }
}
