using System;
using System.Threading.Tasks;
using Nemeio.Core.PackageUpdater.Downloader;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.PackageUpdater.Updatable.States;
using Nemeio.Core.Tools;

namespace Nemeio.Core.PackageUpdater.Updatable
{
    public interface IUpdatable : IStoppable
    {
        UpdateState State { get; }
        bool IsMandatoryUpdate { get; }

        event EventHandler<UpdateStateChangedEventArgs> OnUpdateStateChanged;
        event EventHandler<UpdateFinishedEventArgs> OnUpdateFinished;

        void AddDownloadableDependency(DownloadablePackageInformation package);
        Task DownloadDependenciesAsync(IPackagesDownloader downloader);
        Task UpdateAsync();
    }
}
