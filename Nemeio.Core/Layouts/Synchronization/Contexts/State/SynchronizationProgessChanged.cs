namespace Nemeio.Core.Layouts.Synchronization.Contexts.State
{
    public sealed class SynchronizationProgessChanged
    {
        public uint Progress { get; private set; }
        public uint Size { get; private set; }

        public SynchronizationProgessChanged(uint progress, uint size)
        {
            Progress = progress;
            Size = size;
        }
    }
}
