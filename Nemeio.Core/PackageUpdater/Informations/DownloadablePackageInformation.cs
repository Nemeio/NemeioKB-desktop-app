using System;
using System.IO;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater.Informations
{
    public class DownloadablePackageInformation : PackageInformation
    {
        private const string InstallersFolderName = "installers";
        private const string ApplicationInstallersFolderName = "application";

        public string Checksum { get; protected set; }
        public Uri Url { get; protected set; }

        public DownloadablePackageInformation(Version version, string checksum, Uri url)
            : base(version)
        {
            if (string.IsNullOrWhiteSpace(checksum))
            {
                throw new ArgumentNullException(nameof(checksum));
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Version = version;
            Url = url;
            Checksum = checksum;
        }

        public override string GetPackageFileName()
        {
            var fileName = Path.GetFileName(Url.AbsolutePath);

            return fileName;
        }

        public override string FindPackageContainerFolderPath(IDocument document)
        {
            var path = Path.Combine(
                Path.Combine(document.UserNemeioFolder, InstallersFolderName),
                ApplicationInstallersFolderName
            );

            return path;
        }

        public override string ToString() => $"DownloadablePackageInformation: Url=<{Url}>, Version=<{Version}>, Checksum=<{Checksum}>";
    }
}
