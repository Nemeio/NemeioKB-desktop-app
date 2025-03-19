using System;

namespace Nemeio.Core.Layouts.Synchronization.Contexts.State
{
    public sealed class SynchronizationContextState : ISynchronizationContextState
    {
        public SynchronizationState State { get; private set; } = SynchronizationState.Hold;
        public uint CurrentProgress { get; private set; }
        public uint ModificationCount { get; private set; }

        public event EventHandler StateChanged;
        public event EventHandler<SynchronizationProgessChanged> ProgressionChanged;

        public void StartSync(uint nbModification)
        {
            ModificationCount = nbModification;
            State = SynchronizationState.Start;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Progress()
        {
            if (State != SynchronizationState.InProgress)
            {
                State = SynchronizationState.InProgress;
                StateChanged?.Invoke(this, EventArgs.Empty);
            }

            CurrentProgress += 1;
            ProgressionChanged?.Invoke(this, new SynchronizationProgessChanged(CurrentProgress, ModificationCount));
        }

        public void EndSync()
        {
            State = SynchronizationState.End;
            StateChanged?.Invoke(this, EventArgs.Empty);

            CurrentProgress = 0;
            ModificationCount = 0;
        }
    }
}
