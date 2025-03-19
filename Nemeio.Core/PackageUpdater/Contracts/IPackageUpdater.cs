using System;
using System.Threading.Tasks;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Core.Tools.StateMachine;

namespace Nemeio.Core.PackageUpdater
{
    public interface IPackageUpdater : IStateMachine<PackageUpdateState>
    {
        bool PendingUpdates { get; }
        IUpdatable Component { get; }

        event EventHandler OnUpdateAvailable;

        Task CheckUpdatesAsync();
        Task DownloadPendingUpdateAsync();
        Task InstallUpdateAsync();
    }
}
