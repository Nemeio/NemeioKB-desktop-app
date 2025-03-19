using System;
using System.Threading.Tasks;
using Nemeio.Core.Connectivity;
using Nemeio.Core.Keyboard;
using Nemeio.Core.PackageUpdater.Downloader;
using Nemeio.Core.PackageUpdater.Updatable;

namespace Nemeio.Core.PackageUpdater.Tools
{
    public interface IPackageUpdaterTool
    {
        event EventHandler<DownloadFinishedEventArgs> OnDownloadFinished;
        event EventHandler<InternetIsAvailableEventArgs> OnInternetStateComputed;
        event EventHandler<ApplicationUpdateCheckedEventArgs> OnApplicationUpdateChecked;
        event EventHandler<FirmwareUpdateCheckedEventArgs> OnFirmwareUpdateChecked;
        event EventHandler<InstallFinishedEventArgs> OnEmbeddedInstallFinished;
        event EventHandler<SoftwareInstallFinishedEventArgs> OnSoftwareInstallFinished;

        Task CheckSoftwareUpdateAsync();
        Task DownloadDependenciesAsync(IUpdatable updatable, IPackagesDownloader downloader);
        Task CheckInternetConnectionAsync(INetworkConnectivityChecker checker);
        Task CheckApplicationUpdateAsync(IPackageUpdateChecker updateChecker);
        Task CheckFirmwareUpdateAsync(IKeyboardController keyboardController);
        Task InstallAsync(IUpdatable updatable);
    }
}
