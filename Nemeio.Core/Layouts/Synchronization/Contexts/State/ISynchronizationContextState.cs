using System;

namespace Nemeio.Core.Layouts.Synchronization.Contexts.State
{
    public interface ISynchronizationContextState
    {
        SynchronizationState State { get; }
        uint CurrentProgress { get; }
        uint ModificationCount { get; }

        event EventHandler StateChanged;

        event EventHandler<SynchronizationProgessChanged> ProgressionChanged;

        void StartSync(uint nbModification);
        void Progress();
        void EndSync();
    }
}
