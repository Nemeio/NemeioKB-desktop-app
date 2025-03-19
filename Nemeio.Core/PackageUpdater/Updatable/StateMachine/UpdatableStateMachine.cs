using System.Collections.Generic;
using System.Diagnostics;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.PackageUpdater.Downloader;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.PackageUpdater.Updatable.States;
using Nemeio.Core.Tools.StateMachine;
using Stateless;

namespace Nemeio.Core.PackageUpdater.Updatable.StateMachine
{
    public sealed class UpdatableStateMachine : StateMachine<UpdateState>
    {
        private readonly StateMachine<UpdateState, UpdatableTrigger>.TriggerWithParameters<PackagesDownloaderInProgressEventArgs> _downloadingTrigger;
        private readonly StateMachine<UpdateState, UpdatableTrigger>.TriggerWithParameters<DownloadablePackageInformation> _addDependencyTrigger;
        private readonly StateMachine<UpdateState, UpdatableTrigger>.TriggerWithParameters<UpdateProgress> _updating;
        private readonly StateMachine<UpdateState, UpdatableTrigger> _stateMachine;

        private readonly UpdateIdleState _idleState;
        private readonly UpdateDownloadPendingState _downloadPendingState;
        private readonly UpdateDownloadingState _downloadingState;
        private readonly UpdatePendingState _updatePendingState;
        private readonly UpdateInProgressState _updatingState;
        private readonly UpdateFailState _updateFailed;
        private readonly UpdateSuccessState _updateSucceed;

        public UpdatableStateMachine()
        {
            _idleState = new UpdateIdleState();
            _downloadPendingState = new UpdateDownloadPendingState();
            _downloadingState = new UpdateDownloadingState();
            _updatePendingState = new UpdatePendingState();
            _updatingState = new UpdateInProgressState();
            _updateFailed = new UpdateFailState();
            _updateSucceed = new UpdateSuccessState();

            _stateMachine = new StateMachine<UpdateState, UpdatableTrigger>(_idleState);

            _downloadingTrigger = _stateMachine.SetTriggerParameters<PackagesDownloaderInProgressEventArgs>(UpdatableTrigger.Download);
            _addDependencyTrigger = _stateMachine.SetTriggerParameters<DownloadablePackageInformation>(UpdatableTrigger.AddDependency);
            _updating = _stateMachine.SetTriggerParameters<UpdateProgress>(UpdatableTrigger.Updating);

            _stateMachine.Configure(_idleState)
                .Permit(UpdatableTrigger.StartUpdate, _updatingState)
                .Permit(UpdatableTrigger.AddDependency, _downloadPendingState);

            _stateMachine.Configure(_downloadPendingState)
                .OnEntryFrom(_addDependencyTrigger, (trigger) => _downloadPendingState.AddDependency(trigger))
                .PermitReentry(UpdatableTrigger.AddDependency)
                .Permit(UpdatableTrigger.Download, _downloadingState);

            _stateMachine.Configure(_downloadingState)
                .OnEntryFrom(_downloadingTrigger, (trigger) => _downloadingState.UpdateDownloadProgress(trigger.FileCount, trigger.CurrentFileIndex, trigger.Percent))
                .PermitReentry(UpdatableTrigger.Download)
                .Permit(UpdatableTrigger.DownloadFinished, _updatePendingState)
                .Permit(UpdatableTrigger.DownloadFailed, _idleState);

            _stateMachine.Configure(_updatePendingState)
                .Permit(UpdatableTrigger.StartUpdate, _updatingState);

            _stateMachine.Configure(_updatingState)
                .PermitReentry(UpdatableTrigger.Updating)
                .OnEntryFrom(_updating, (trigger) => _updatingState.UpdateInstallProgress(trigger))
                .Permit(UpdatableTrigger.UpdateFailed, _updateFailed)
                .Permit(UpdatableTrigger.UpdateSucceed, _updateSucceed)
                .Ignore(UpdatableTrigger.StartUpdate);

            _stateMachine.OnTransitioned((transition) =>
            {
                State = transition.Destination;

                Debug.WriteLine($"UpdatableStateMachine.OnTransitioned: {transition.Source} -> {transition.Destination} via {transition.Trigger}({string.Join(", ", transition.Parameters)})");
            });
        }

        #region Update

        public void StartUpdate() => _stateMachine.Fire(UpdatableTrigger.StartUpdate);

        public void Updating(UpdateComponent component, int percent) => _stateMachine.Fire(_updating, new UpdateProgress(component, percent));

        public void UpdateFailed() => _stateMachine.Fire(UpdatableTrigger.UpdateFailed);

        public void UpdateSucceed() => _stateMachine.Fire(UpdatableTrigger.UpdateSucceed);

        #endregion

        #region Download

        public void AddDependency(DownloadablePackageInformation package) => _stateMachine.Fire(_addDependencyTrigger, package);

        public IList<DownloadablePackageInformation> StartDownload()
        {
            var eventArgs = new PackagesDownloaderInProgressEventArgs(0, 0, 0, 0, 0);

            _stateMachine.Fire(_downloadingTrigger, eventArgs);

            return _downloadPendingState.Packages;
        }

        public void Download(PackagesDownloaderInProgressEventArgs eventArgs) => _stateMachine.Fire(_downloadingTrigger, eventArgs);

        public void DownloadFailed() => _stateMachine.Fire(UpdatableTrigger.DownloadFailed);

        public void DownloadFinished() => _stateMachine.Fire(UpdatableTrigger.DownloadFinished);

        #endregion
    }
}