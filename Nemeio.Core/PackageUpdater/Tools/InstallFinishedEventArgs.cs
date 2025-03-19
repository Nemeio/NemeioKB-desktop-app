using System;

namespace Nemeio.Core.PackageUpdater.Tools
{
    public class InstallFinishedEventArgs
    {
        public Exception Exception { get; private set; }
        public InstallFinishedEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
