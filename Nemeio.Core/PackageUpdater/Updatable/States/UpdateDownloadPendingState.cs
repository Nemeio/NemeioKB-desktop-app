using System.Collections.Generic;
using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Core.PackageUpdater.Updatable.States
{
    public class UpdateDownloadPendingState : UpdateState
    {
        private readonly IList<DownloadablePackageInformation> _packages;

        public IList<DownloadablePackageInformation> Packages => _packages;

        public UpdateDownloadPendingState()
        {
            _packages = new List<DownloadablePackageInformation>();
        }

        public void AddDependency(DownloadablePackageInformation dependency)
        {
            _packages.Add(dependency);
        }
    }
}
