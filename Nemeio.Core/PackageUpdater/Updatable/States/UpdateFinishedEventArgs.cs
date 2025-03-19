using Nemeio.Core.Errors;

namespace Nemeio.Core.PackageUpdater.Updatable.States
{
    public class UpdateFinishedEventArgs
    {
        public ErrorCode ErrorCode { get; private set; }

        public UpdateFinishedEventArgs(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
