namespace Nemeio.Core.PackageUpdater.Tools
{
    public sealed class DownloadFinishedEventArgs
    {
        public bool IsSuccess { get; private set; }

        public DownloadFinishedEventArgs(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}
