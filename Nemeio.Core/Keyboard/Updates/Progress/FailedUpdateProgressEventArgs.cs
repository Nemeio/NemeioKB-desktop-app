using Nemeio.Core.Errors;

namespace Nemeio.Core.Keyboard.Updates.Progress
{
    public sealed class FailedUpdateProgressEventArgs : UpdateProgressEventArgs
    {
        public ErrorCode Error { get; private set; }

        public FailedUpdateProgressEventArgs(UpdateComponent component, ErrorCode errorCode) 
            : base(component, UpdateStatusType.Failed)
        {
            Error = errorCode;
        }
    }
}
