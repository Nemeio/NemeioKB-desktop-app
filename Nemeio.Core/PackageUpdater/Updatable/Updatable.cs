using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.PackageUpdater.Downloader;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.PackageUpdater.Strategies;
using Nemeio.Core.PackageUpdater.Updatable.StateMachine;
using Nemeio.Core.PackageUpdater.Updatable.States;
using Nemeio.Core.Tools.StateMachine;
using Nemeio.Core.Tools.Stoppable;

namespace Nemeio.Core.PackageUpdater.Updatable
{
    public enum UpdatableTrigger
    {
        AddDependency,

        StartUpdate,
        Updating,
        UpdateSucceed,
        UpdateFailed,

        Download,
        DownloadFailed,
        DownloadFinished
    }

    public abstract class Updatable : Stoppable, IUpdatable
    {
        private readonly IInstallStrategy _installStrategy;
        private readonly SemaphoreSlim _downloadSempahore = new SemaphoreSlim(1, 1);

        protected readonly ILogger _logger;
        protected readonly UpdatableStateMachine _stateMachine;

        public virtual bool IsMandatoryUpdate { get; }
        public UpdateState State => _stateMachine.State;

        public event EventHandler<UpdateStateChangedEventArgs> OnUpdateStateChanged;
        public event EventHandler<UpdateFinishedEventArgs> OnUpdateFinished;

        public Updatable(ILogger logger, IInstallStrategy strategy) 
            : base(false)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _installStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _stateMachine = new UpdatableStateMachine();
            _stateMachine.OnStateChanged += StateMachine_OnStateChanged;
        }

        public override void Stop()
        {
            base.Stop();

            OnUpdateFinished?.Invoke(this, new UpdateFinishedEventArgs(ErrorCode.Success));
        }

        public void AddDownloadableDependency(DownloadablePackageInformation package)
        {
            _logger.LogInformation($"Add dependency <{package}>");

            _stateMachine.AddDependency(package);
        }

        public async Task DownloadDependenciesAsync(IPackagesDownloader downloader)
        {
            if (downloader == null)
            {
                throw new ArgumentNullException(nameof(downloader));
            }

            _logger.LogInformation($"Download dependencies ...");

            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;

            try
            {
                var packages = _stateMachine.StartDownload();

                await downloader.Download(packages);

                _logger.LogInformation($"Download dependencies with success");

                _stateMachine.DownloadFinished();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Download dependencies failed");

                _stateMachine.DownloadFailed();
            }
            finally
            {
                downloader.DownloadProgressChanged -= Downloader_DownloadProgressChanged;
            }
        }

        public virtual async Task UpdateAsync()
        {
            try
            {
                _logger.LogInformation($"Start updating ...");

                _stateMachine.StartUpdate();

                await _installStrategy.InstallAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Updating failed");

                RaiseUpdateFinishedWithError();

                throw;
            }
        }

        protected void RaiseUpdateFinishedWithSuccess()
        {
            _stateMachine.UpdateSucceed();
            RaiseUpdateFinished(ErrorCode.Success);
        }

        protected void RaiseUpdateFinishedWithError()
        {
            _stateMachine.UpdateFailed();
            RaiseUpdateFinished(ErrorCode.AclKeyboardResponseFirmwareUpdateFailed);
        }

        protected void RaiseUpdateFinished(ErrorCode errorCode) => OnUpdateFinished?.Invoke(this, new UpdateFinishedEventArgs(errorCode));

        private void StateMachine_OnStateChanged(object sender, OnStateChangedEventArgs<UpdateState> e)
        {
            var eventArgs = new UpdateStateChangedEventArgs(e.PreviousState, e.State);

            OnUpdateStateChanged?.Invoke(this, eventArgs);
        }

        private void Downloader_DownloadProgressChanged(object sender, PackagesDownloaderInProgressEventArgs e)
        {
            _downloadSempahore.Wait();

            try
            {
                _logger.LogInformation($"Download progress changed <{e.Percent}>");

                _stateMachine.Download(e);
            }
            finally
            {
                _downloadSempahore.Release();
            }
        }
    }
}
