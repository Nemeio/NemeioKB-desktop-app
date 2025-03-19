using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Connection;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keyboard.SerialNumber;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Keyboard.Version;
using Nemeio.Core.PackageUpdater.Strategies;
using Nemeio.Core.Tools;

namespace Nemeio.Core.PackageUpdater.Updatable
{
    public class UpdatableKeyboard : Updatable
    {
        private sealed class VersionableNemeioProxy : KeyboardProxy, IVersionHolder
        {
            public VersionableNemeioProxy(INemeio nemeio)
                : base(nemeio) { }
        }

        private readonly IKeyboardController _keyboardController;
        private readonly FirmwareUpdatableNemeioProxy _firmwareUpdatableProxy;
        private readonly ITimer _timeoutTimer;
        private readonly FirmwareManifest _manifest;

        private VersionableNemeioProxy _versionableProxy;
        private NemeioSerialNumber _keyboardSerialNumber;

        public override bool IsMandatoryUpdate => true;
        public CommunicationType CommunicationType => _firmwareUpdatableProxy.CommunicationType;

        public UpdatableKeyboard(ILoggerFactory loggerFactory, IKeyboardController keyboardController, FirmwareUpdatableNemeioProxy proxy, IInstallStrategy installStrategy, ITimer timer, FirmwareManifest manifest)
             : base(loggerFactory.CreateLogger<UpdatableKeyboard>(), installStrategy)
        {
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _firmwareUpdatableProxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _timeoutTimer = timer ?? throw new ArgumentNullException(nameof(timer));
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        public override async Task UpdateAsync()
        {
            _keyboardSerialNumber = _keyboardController.Nemeio.SerialNumber;
            _keyboardController.OnKeyboardConnected += KeyboardController_OnKeyboardConnected;
            _keyboardController.OnKeyboardDisconnecting += KeyboardController_OnKeyboardDisconnecting;

            _firmwareUpdatableProxy.OnUpdateProgressChanged += Proxy_OnUpdateProgressChanged;
            _firmwareUpdatableProxy.OnRollbackProgressChanged += Proxy_OnRollbackProgressChanged;
            _firmwareUpdatableProxy.OnUpdateFailed += Proxy_OnUpdateFailed;

            await base.UpdateAsync();
        }

        #region Stoppable 

        public override void Stop()
        {
            base.Stop();

            if (_firmwareUpdatableProxy != null)
            {
                _firmwareUpdatableProxy.OnUpdateProgressChanged -= Proxy_OnUpdateProgressChanged;
                _firmwareUpdatableProxy.OnRollbackProgressChanged -= Proxy_OnRollbackProgressChanged;
                _firmwareUpdatableProxy.OnUpdateFailed -= Proxy_OnUpdateFailed;
            }
        }

        #endregion

        #region Firmware Updatable Proxy Events

        private void Proxy_OnUpdateProgressChanged(object sender, Keyboard.Updates.Progress.InProgressUpdateProgressEventArgs e)
        {
            _logger.LogInformation($"Proxy <{this}> updating progress changed : Component=<{e.Component}> Percent=<{e.Progress}>");

            _stateMachine.Updating(e.Component, e.Progress);
        }

        private void Proxy_OnRollbackProgressChanged(object sender, Keyboard.Updates.Progress.RollbackUpdateProgressEventArgs e)
        {
            //  TODO [KSB] BLDLCK-2919 : Manage progress for rollback
        }

        private void Proxy_OnUpdateFailed(object sender, Keyboard.Updates.Progress.FailedUpdateProgressEventArgs e)
        {
            _logger.LogInformation($"Proxy <{this}> notify update failed for Component=<{e.Component}> Error=<{e.Error}>");

            RaiseUpdateFinishedWithError();
        }

        #endregion

        #region Versionable Proxy Event

        private void VersionableProxy_OnStateChanged(object sender, StateChangedEventArgs e)
        {
            if (_versionableProxy != null && _versionableProxy.Is(e.Nemeio) && _versionableProxy.State == NemeioState.Ready)
            {
                _logger.LogInformation($"Keyboard <{this}> appear with state ready, we stop timeout timer");

                //  Right keyboard connected
                //  We stop timer
                _timeoutTimer?.Stop();

                CheckInstallSucceed();
            }
        }

        #endregion

        #region KeyboardController Events

        private void KeyboardController_OnKeyboardConnected(object sender, KeyboardStatusChangedEventArgs e)
        {
            if (_keyboardSerialNumber != null)
            {
                var proxy = KeyboardProxy.CastTo<VersionableNemeioProxy>(e.Keyboard);
                if (proxy != null)
                {
                    _versionableProxy = proxy;
                    _versionableProxy.OnStateChanged += VersionableProxy_OnStateChanged; ;
                }
            }
        }

        private void KeyboardController_OnKeyboardDisconnecting(object sender, KeyboardStatusChangedEventArgs e)
        {
            //  If we have a proxy we start timer
            if (_versionableProxy != null && _versionableProxy.Is(e.Keyboard))
            {
                _logger.LogInformation($"Updatable keyboard <{this}> is disconnected, we start timeout timer ...");

                _timeoutTimer.OnTimeElapsed += TimeoutTimer_OnTimeElapsed;
                _timeoutTimer.Start();
            }
        }

        #endregion

        #region Timer Events

        private void TimeoutTimer_OnTimeElapsed(object sender, EventArgs e)
        {
            //  Time is elasped
            //  Change state to error
            _logger.LogInformation($"Updatable keyboard <{this}> timeout timer elasped, update failed");

            RaiseUpdateFinishedWithError();
        }

        #endregion

        private void CheckInstallSucceed()
        {
            _logger.LogInformation($"Check update on keyboard <{this}> ...");

            //  Check Cpu
            var cpuVersion = _versionableProxy.Versions.Stm32;
            var cpuUpToDate = _manifest.Cpu.Version.ToString().Equals(cpuVersion.ToString());

            //  Check Nrf
            var nrfVersion = _versionableProxy.Versions.Nrf;
            var nrfUpToDate = _manifest.BluetoothLE.Version.ToString().Equals(nrfVersion.ToString());

            //  Check Ite
            var iteVersion = _versionableProxy.Versions.Ite;
            var iteUpToDate = _manifest.Ite.Version.ToString().Equals(iteVersion.ToString());

            if (cpuUpToDate && nrfUpToDate && iteUpToDate)
            {
                _logger.LogInformation($"Keyboard <{this}> update works successfully");

                RaiseUpdateFinishedWithSuccess();
            }
            else
            {
                _logger.LogWarning($"Keyboard <{this}> update failed, cpu: <{cpuUpToDate}>, nrf: <{nrfUpToDate}>, ite: <{iteUpToDate}>");

                RaiseUpdateFinishedWithError();
            }

            _keyboardSerialNumber = null;

            if (_versionableProxy != null)
            {
                _versionableProxy.OnStateChanged -= VersionableProxy_OnStateChanged;
                _versionableProxy = null;
            }
        }

        public override string ToString() => $"Updatable keyboard for <{_firmwareUpdatableProxy.SerialNumber}> on <{_firmwareUpdatableProxy.Identifier}>";
    }
}
