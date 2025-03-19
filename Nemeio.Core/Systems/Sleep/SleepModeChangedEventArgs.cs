using System;

namespace Nemeio.Core.Systems.Sleep
{
    public sealed class SleepModeChangedEventArgs : EventArgs
    {
        public SleepMode Mode { get; private set; }

        public SleepModeChangedEventArgs(SleepMode mode)
        {
            Mode = mode;
        }
    }
}
