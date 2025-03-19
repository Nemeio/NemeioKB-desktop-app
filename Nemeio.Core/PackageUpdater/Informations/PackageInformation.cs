using System;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater.Informations
{
    public abstract class PackageInformation
    {
        public Version Version { get; protected set; }

        public PackageInformation(Version version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Version = version;
        }

        public abstract string GetPackageFileName();
        public abstract string FindPackageContainerFolderPath(IDocument document);
    }
}
