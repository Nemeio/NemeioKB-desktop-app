using System;

namespace Nemeio.Core.Systems.Sleep
{
    public interface ISleepModeAdapter
    {
        event EventHandler<SleepModeChangedEventArgs> OnSleepModeChanged;
    }
}
