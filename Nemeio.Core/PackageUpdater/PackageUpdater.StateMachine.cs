using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Core.Tools.StateMachine;
using Stateless;

namespace Nemeio.Core.PackageUpdater
{
    public partial class PackageUpdater : StateMachine<PackageUpdateState>
    {
        private enum PackageUpdateTrigger
        {
            RequestCheckInstall,
            NoApplicationUpdateFound,
            ApplicationInstallationFinished,
            RequestCheckUpdates,
            CheckInternetConnection,
            InternetStateChanged,
            ApplicationUpdateReceivedResponse,
            FirmwareUpdateReceivedResponse,
            RequestUpdate,
            RequestDownload,
            UpdateFinished,
            DownloadFinished,
            NoUsbConnected,
            KeyboardDisconnected
        }

        private StateMachine<PackageUpdateState, PackageUpdateTrigger> _stateMachine;
        private StateMachine<PackageUpdateState, PackageUpdateTrigger>.TriggerWithParameters<bool> _internetStateChanged;
        private StateMachine<PackageUpdateState, PackageUpdateTrigger>.TriggerWithParameters<IUpdatable> _applicationUpdateFoundTrigger;
        private StateMachine<PackageUpdateState, PackageUpdateTrigger>.TriggerWithParameters<IUpdatable> _firmwareUpdateFoundTrigger;
        private StateMachine<PackageUpdateState, PackageUpdateTrigger>.TriggerWithParameters<bool> _downloadFinishedTrigger;
        private StateMachine<PackageUpdateState, PackageUpdateTrigger>.TriggerWithParameters<Exception> _updateFinishedTrigger;
        private StateMachine<PackageUpdateState, PackageUpdateTrigger>.TriggerWithParameters<bool> _applicationInstallationFinished;

        private void ConfigureStateMachine()
        {
            _stateMachine = new StateMachine<PackageUpdateState, PackageUpdateTrigger>(PackageUpdateState.Idle);

            _internetStateChanged = _stateMachine.SetTriggerParameters<bool>(PackageUpdateTrigger.InternetStateChanged);
            _applicationUpdateFoundTrigger = _stateMachine.SetTriggerParameters<IUpdatable>(PackageUpdateTrigger.ApplicationUpdateReceivedResponse);
            _firmwareUpdateFoundTrigger = _stateMachine.SetTriggerParameters<IUpdatable>(PackageUpdateTrigger.FirmwareUpdateReceivedResponse);
            _downloadFinishedTrigger = _stateMachine.SetTriggerParameters<bool>(PackageUpdateTrigger.DownloadFinished);
            _updateFinishedTrigger = _stateMachine.SetTriggerParameters<Exception>(PackageUpdateTrigger.UpdateFinished);
            _applicationInstallationFinished = _stateMachine.SetTriggerParameters<bool>(PackageUpdateTrigger.ApplicationInstallationFinished);

            _stateMachine.Configure(PackageUpdateState.Idle)
                .OnEntry(() => Component = null)
                .Permit(PackageUpdateTrigger.RequestCheckInstall, PackageUpdateState.CheckApplicationInstallation)
                .Ignore(PackageUpdateTrigger.KeyboardDisconnected);

            _stateMachine.Configure(PackageUpdateState.UpdateChecking)
                .Ignore(PackageUpdateTrigger.RequestCheckUpdates)
                .Ignore(PackageUpdateTrigger.RequestCheckInstall)
                .Ignore(PackageUpdateTrigger.KeyboardDisconnected);

            _stateMachine.Configure(PackageUpdateState.CheckApplicationInstallation)
                .SubstateOf(PackageUpdateState.UpdateChecking)
                .OnEntryAsync(() => _tool.CheckSoftwareUpdateAsync())
                .Permit(PackageUpdateTrigger.NoApplicationUpdateFound, PackageUpdateState.CheckInternetConnection)
                .PermitDynamic(_applicationInstallationFinished, (withSuccess) =>
                {
                    if (withSuccess)
                    {
                        return PackageUpdateState.UpdateSucceed;
                    }
                    else
                    {
                        return PackageUpdateState.UpdateFailed;
                    }
                });

            _stateMachine.Configure(PackageUpdateState.CheckInternetConnection)
                .SubstateOf(PackageUpdateState.UpdateChecking)
                .OnEntryAsync(() => _tool.CheckInternetConnectionAsync(_networkConnectivityChecker))
                .PermitDynamic(_internetStateChanged, (isAvailable) =>
                {
                    if (isAvailable)
                    {
                        return PackageUpdateState.CheckApplicationUpdate;
                    }
                    else
                    {
                        return PackageUpdateState.Idle;
                    }
                })
                .Ignore(PackageUpdateTrigger.KeyboardDisconnected);

            _stateMachine.Configure(PackageUpdateState.CheckApplicationUpdate)
                .SubstateOf(PackageUpdateState.UpdateChecking)
                .OnEntry(() => _tool.CheckApplicationUpdateAsync(_updateChecker))
                .PermitDynamic(_applicationUpdateFoundTrigger, (updatable) =>
                {
                    if (updatable != null)
                    {
                        return PackageUpdateState.DownloadPending;
                    }
                    else
                    {
                        return PackageUpdateState.CheckFirmwareUpdate;
                    }
                })
                .Ignore(PackageUpdateTrigger.KeyboardDisconnected)
                .Ignore(PackageUpdateTrigger.RequestCheckInstall);

            _stateMachine.Configure(PackageUpdateState.CheckFirmwareUpdate)
                .SubstateOf(PackageUpdateState.UpdateChecking)
                .OnEntry(() => _tool.CheckFirmwareUpdateAsync(_keyboardController))
                .PermitDynamic(_firmwareUpdateFoundTrigger, (updatable) =>
                {
                    if (updatable != null)
                    {
                        return PackageUpdateState.UpdatePending;
                    }
                    else
                    {
                        return PackageUpdateState.Idle;
                    }
                })
                .Permit(PackageUpdateTrigger.KeyboardDisconnected, PackageUpdateState.Idle);

            _stateMachine.Configure(PackageUpdateState.DownloadPending)
                .SubstateOf(PackageUpdateState.Download)
                .OnEntryFrom(_applicationUpdateFoundTrigger, (updatable) => Component = updatable)
                .Permit(PackageUpdateTrigger.RequestUpdate, PackageUpdateState.ApplyUpdate)
                .Permit(PackageUpdateTrigger.RequestDownload, PackageUpdateState.Downloading)
                .Ignore(PackageUpdateTrigger.RequestCheckUpdates)
                .Ignore(PackageUpdateTrigger.RequestCheckInstall);

            _stateMachine.Configure(PackageUpdateState.UpdatePending)
                .SubstateOf(PackageUpdateState.Updating)
                .OnEntryFrom(_firmwareUpdateFoundTrigger, (updatable) => Component = updatable)
                .PermitIf(PackageUpdateTrigger.RequestUpdate, PackageUpdateState.ApplyUpdate, () => !IsKeyboardUpdate() || (IsKeyboardUpdate() && IsUsbKeyboard()))
                .PermitIf(PackageUpdateTrigger.RequestUpdate, PackageUpdateState.WaitUsbKeyboard, () => IsKeyboardUpdate() && !IsUsbKeyboard())
                .Permit(PackageUpdateTrigger.KeyboardDisconnected, PackageUpdateState.Idle)
                .Ignore(PackageUpdateTrigger.RequestCheckUpdates)
                .Ignore(PackageUpdateTrigger.RequestCheckInstall);

            _stateMachine.Configure(PackageUpdateState.WaitUsbKeyboard)
                .SubstateOf(PackageUpdateState.Updating)
                .PermitIf(PackageUpdateTrigger.RequestUpdate, PackageUpdateState.ApplyUpdate, () => !IsKeyboardUpdate() || (IsKeyboardUpdate() && IsUsbKeyboard()))
                .PermitReentryIf(PackageUpdateTrigger.RequestUpdate, () => IsKeyboardUpdate() && !IsUsbKeyboard())
                .Permit(PackageUpdateTrigger.KeyboardDisconnected, PackageUpdateState.Idle)
                .Ignore(PackageUpdateTrigger.RequestCheckUpdates)
                .Ignore(PackageUpdateTrigger.RequestCheckInstall);

            _stateMachine.Configure(PackageUpdateState.ApplyUpdate)
                .SubstateOf(PackageUpdateState.Updating)
                .OnEntry(() => _tool.InstallAsync(Component))
                .PermitDynamic(_updateFinishedTrigger, (exception) =>
                {
                    if (exception != null)
                    {
                        return PackageUpdateState.UpdateFailed;
                    }
                    else
                    {
                        return PackageUpdateState.UpdateSucceed;
                    }
                })
                .Ignore(PackageUpdateTrigger.RequestCheckUpdates)
                .Ignore(PackageUpdateTrigger.RequestCheckInstall)
                .Ignore(PackageUpdateTrigger.KeyboardDisconnected);

            _stateMachine.Configure(PackageUpdateState.Downloading)
                .SubstateOf(PackageUpdateState.Download)
                .OnEntry(() => _tool.DownloadDependenciesAsync(Component, _downloader))
                .PermitDynamic(_downloadFinishedTrigger, (success) =>
                {
                    if (success)
                    {
                        return PackageUpdateState.UpdatePending;
                    }
                    else
                    {
                        return PackageUpdateState.UpdateFailed;
                    }
                })
                .Ignore(PackageUpdateTrigger.RequestCheckUpdates)
                .Ignore(PackageUpdateTrigger.RequestCheckInstall)
                .Ignore(PackageUpdateTrigger.KeyboardDisconnected);

            _stateMachine.Configure(PackageUpdateState.UpdateFailed)
                .OnEntry(OnUpdateFinished)
                .Permit(PackageUpdateTrigger.RequestCheckUpdates, PackageUpdateState.UpdateChecking)
                .Permit(PackageUpdateTrigger.RequestCheckInstall, PackageUpdateState.CheckApplicationInstallation);

            _stateMachine.Configure(PackageUpdateState.UpdateSucceed)
                .OnEntry(OnUpdateFinished)
                .Permit(PackageUpdateTrigger.RequestCheckUpdates, PackageUpdateState.UpdateChecking)
                .Permit(PackageUpdateTrigger.RequestCheckInstall, PackageUpdateState.CheckApplicationInstallation);

            _stateMachine.OnTransitionCompleted((transition) =>
            {
                State = transition.Destination;
                _keyboardController.RaisePackageUpdaterStateChanged(State);
                _logger.LogInformation($"OnTransitionCompleted: {transition.Source} -> {transition.Destination} via {transition.Trigger}({string.Join(", ", transition.Parameters)})");
            });
        }

        private void OnUpdateFinished()
        {
            Component = null;

            _applicationSettingsManager.UpdateTo = null;
        }

        private bool IsKeyboardUpdate()
        {
            bool isKeyboard = Component != null && Component is UpdatableKeyboard;

            return isKeyboard;
        }

        private bool IsUsbKeyboard()
        {
            bool isUsb = false;

            if (Component != null && Component is UpdatableKeyboard keyboard)
            {
                isUsb = keyboard.CommunicationType == CommunicationType.Serial;
            }

            return isUsb;
        }

        private Task InternalCheckUpdatesAsync() => _stateMachine.FireAsync(PackageUpdateTrigger.RequestCheckUpdates);

        private void CheckInternetConnection() => _stateMachine.Fire(PackageUpdateTrigger.CheckInternetConnection);

        private void InternetStateChanged(bool isAvailable) => _stateMachine.Fire(_internetStateChanged, isAvailable);

        private void ApplicationUpdateIsAvailable(IUpdatable updatable) => _stateMachine.Fire(_applicationUpdateFoundTrigger, updatable);

        private void FirmwareUpdateChecked(IUpdatable updatable) => _stateMachine.Fire(_firmwareUpdateFoundTrigger, updatable);

        private Task InternalUpdateAsync() => _stateMachine.FireAsync(PackageUpdateTrigger.RequestUpdate);

        private Task InternalDownloadAsync() => _stateMachine.FireAsync(PackageUpdateTrigger.RequestDownload);

        private void UpdateFinished(Exception exception) => _stateMachine.Fire(_updateFinishedTrigger, exception);

        private void DownloadFinished(bool withSuccess) => _stateMachine.Fire(_downloadFinishedTrigger, withSuccess);

        private void NoUsbConnected() => _stateMachine.Fire(PackageUpdateTrigger.NoUsbConnected);

        private Task CheckInstallAsync() => _stateMachine.FireAsync(PackageUpdateTrigger.RequestCheckInstall);

        private void NoApplicationInstallationFound() => _stateMachine.Fire(PackageUpdateTrigger.NoApplicationUpdateFound);

        private void ApplicationInstallationFinished(bool withSuccess) => _stateMachine.Fire(_applicationInstallationFinished, withSuccess);

        private void KeyboardDisconnected()
        {
            if (Component != null && Component is UpdatableKeyboard)
            {
                _stateMachine.Fire(PackageUpdateTrigger.KeyboardDisconnected);
            }
        }
    }
}
