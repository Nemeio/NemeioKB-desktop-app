namespace Nemeio.Presentation.Menu.Synchronization
{
    public sealed class SynchronizationProgress
    {
        public uint Progress { get; private set; }
        public uint Size { get; private set; }

        public SynchronizationProgress(uint progress, uint size)
        {
            Progress = progress;
            Size = size;
        }
    }
}
