using System;
using Nemeio.Core.Keyboard.Nemeios;

namespace Nemeio.Core.Keyboard.State
{
    public sealed class StateChangedEventArgs : EventArgs
    {
        public NemeioState State { get; private set; }
        public INemeio Nemeio { get; private set; }

        public StateChangedEventArgs(NemeioState state, INemeio nemeio)
        {
            State = state;
            Nemeio = nemeio ?? throw new ArgumentNullException(nameof(nemeio));
        }
    }
}
