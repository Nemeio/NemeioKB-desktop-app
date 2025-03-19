using System;

namespace Nemeio.Tools.Firmware.PackageBuilder.Builders
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
    }
}
