namespace Nemeio.Core.PackageUpdater.Exceptions
{
    public enum InstallFailReason
    {
        Unknown = 0,
        FileNotFound,
        LaunchFailed,
        KeyboardIsNotConnected,
        NotUsbCommunication
    }

    public class InstallFailedException : PackageUpdaterException 
    {
        public InstallFailReason Reason { get; private set; }
        public InstallFailedException(InstallFailReason reason)
        {
            Reason = reason;
        }
    }
}
