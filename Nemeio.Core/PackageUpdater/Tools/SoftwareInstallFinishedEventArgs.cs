using System;

namespace Nemeio.Core.PackageUpdater.Tools
{
    public sealed class SoftwareInstallFinishedEventArgs : InstallFinishedEventArgs
    {
        public bool HasTryUpdate { get; private set; }

        public SoftwareInstallFinishedEventArgs(bool hasTryUpdate, Exception exception) 
            : base(exception)
        {
            HasTryUpdate = hasTryUpdate;
        }
    }
}
