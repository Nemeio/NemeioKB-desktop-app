namespace Nemeio.Core.Keyboard.Updates.Progress
{
    public sealed class InProgressUpdateProgressEventArgs : UpdateProgressEventArgs
    {
        public int Progress { get; private set; }

        public InProgressUpdateProgressEventArgs(UpdateComponent component, int progress) 
            : base(component, UpdateStatusType.InProgress)
        {
            Progress = progress;
        }
    }
}
