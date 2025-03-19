using System;
using System.Collections.Generic;
using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Core.PackageUpdater.Tools
{
    public sealed class ApplicationUpdateCheckedEventArgs
    {
        public bool Found { get; private set; }
        public IList<DownloadablePackageInformation> Packages { get; private set; }
        public ApplicationUpdateCheckedEventArgs(bool found, IList<DownloadablePackageInformation> packages)
        {
            Found = found;
            Packages = packages ?? throw new ArgumentNullException(nameof(packages));
        }
    }
}
