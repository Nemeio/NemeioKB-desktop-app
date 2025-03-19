namespace Nemeio.Core.Keyboard.Updates.Progress
{
    public sealed class RollbackUpdateProgressEventArgs : UpdateProgressEventArgs
    {
        private const int SuccessStateValue = 0;

        public int State { get; private set; }
        public bool IsSuccess => State == SuccessStateValue;

        public RollbackUpdateProgressEventArgs(UpdateComponent component, int state) 
            : base(component, UpdateStatusType.Rollback)
        {
            State = state;
        }
    }
}
