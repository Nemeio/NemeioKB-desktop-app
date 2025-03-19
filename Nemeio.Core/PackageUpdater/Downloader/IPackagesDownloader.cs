using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Core.PackageUpdater.Downloader
{
    public interface IPackagesDownloader
    {
        event EventHandler<PackagesDownloaderInProgressEventArgs> DownloadProgressChanged;
        Task Download(IList<DownloadablePackageInformation> informations);
    }
}
