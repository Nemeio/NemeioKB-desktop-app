using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Exceptions;
using Nemeio.Core.Keyboard.Configurations;
using Nemeio.Core.Keyboard.Configurations.Add;
using Nemeio.Core.Keyboard.Configurations.Apply;
using Nemeio.Core.Keyboard.Configurations.Changed;
using Nemeio.Core.Keyboard.Configurations.Delete;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.GetLayouts;
using Nemeio.Core.Keyboard.KeepAlive;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Keys;
using Nemeio.Core.Keyboard.LayoutsIds;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Tools;
using Nemeio.Core.Tools.Retry;

namespace Nemeio.Core.Keyboard.Nemeios.Runner
{
    public partial class NemeioRunner : Nemeio, IConfigurationHolder, IAddConfigurationHolder, IDeleteConfigurationHolder, IApplyConfigurationHolder, IKeyPressHolder
    {
        private readonly ITimer _batteryTimer;
        private readonly ITimer _keepAliveTimer;
        private readonly IScreenFactory _screenFactory;

        private readonly IKeepAliveMonitor _keepAliveMonitor;
        private readonly IAddConfigurationMonitor _addConfigurationMonitor;
        private readonly IDeleteConfigurationMonitor _deleteConfigurationMonitor;
        private readonly IApplyConfigurationMonitor _applyConfigurationMonitor;
        private readonly IConfigurationChangedMonitor _configurationChangedMonitor;
        private readonly IKeyPressedMonitor _keyPressedMonitor;
        private readonly IGetLayoutsMonitor _getLayoutMonitor;

        public event EventHandler OnSelectedLayoutChanged;
        public event EventHandler<KeyboardKeyPressedEventArgs> OnKeyPressed;

        public IList<LayoutIdWithHash> LayoutIdWithHashs { get; private set; }
        public LayoutId SelectedLayoutId { get; private set; }
        public IScreen Screen { get; private set; }

        public NemeioRunner(ILoggerFactory loggerFactory, string identifier, System.Version protocolVersion, CommunicationType type, IKeyboardCommandExecutor commandExecutor, IMonitorFactory monitorFactory, IKeyboardCrashLogger crashLogger, ITimer keepAliveTimer, ITimer batteryTimer, IRetryHandler retryHandler, IScreenFactory screenFactory)
            : base(loggerFactory, identifier, protocolVersion, type, commandExecutor, monitorFactory, crashLogger, retryHandler)
        {
            if (monitorFactory == null)
            {
                throw new ArgumentNullException(nameof(monitorFactory));
            }

            _keepAliveTimer = keepAliveTimer ?? throw new ArgumentNullException(nameof(keepAliveTimer));
            _batteryTimer = batteryTimer ?? throw new ArgumentNullException(nameof(batteryTimer));
            _screenFactory = screenFactory ?? throw new ArgumentNullException(nameof(screenFactory));

            _keepAliveMonitor = monitorFactory.CreateKeepAliveMonitor(_commandExecutor);
            _addConfigurationMonitor = monitorFactory.CreateAddConfigurationMonitor(_commandExecutor);
            _deleteConfigurationMonitor = monitorFactory.CreateDeleteConfigurationMonitor(_commandExecutor);
            _applyConfigurationMonitor = monitorFactory.CreateApplyConfigurationMonitor(_commandExecutor);
            _getLayoutMonitor = monitorFactory.CreateGetLayoutsMonitor(_commandExecutor);
            _configurationChangedMonitor = monitorFactory.CreateConfigurationChangedMonitor(_commandExecutor);
            _configurationChangedMonitor.ConfigurationChanged += ConfigurationChangedMonitor_ConfigurationChanged;

            _keyPressedMonitor = monitorFactory.CreateKeyPressedMonitor(_commandExecutor);
            _keyPressedMonitor.OnKeyPressed += KeyPressedMonitor_OnKeyPressed;

            _stateMachine.Configure(NemeioState.Connected)
                .Permit(NemeioTrigger.Initialize, NemeioState.Init)
                .Permit(NemeioTrigger.KeyboardUnplugged, NemeioState.Disconnecting);

            _stateMachine.Configure(NemeioState.Init)
                .SubstateOf(NemeioState.Connected)
                .Permit(NemeioTrigger.KeyboardInitialized, NemeioState.Sync)
                .OnEntryAsync(InitOnEntryAsync);

            _stateMachine.Configure(NemeioState.Sync)
                .SubstateOf(NemeioState.Connected)
                .Ignore(NemeioTrigger.RefreshKeepAlive)
                .Ignore(NemeioTrigger.RefreshBattery)
                .Ignore(NemeioTrigger.StartSync)
                .Permit(NemeioTrigger.LayoutSynced, NemeioState.Ready);

            _stateMachine.Configure(NemeioState.Ready)
                .SubstateOf(NemeioState.Connected)
                .InternalTransitionAsync(NemeioTrigger.RefreshKeepAlive, RefreshKeepAliveAsync)
                .InternalTransitionAsync(NemeioTrigger.RefreshBattery, RefreshBatteryAsync)
                .Permit(NemeioTrigger.StartSync, NemeioState.Sync)
                .Permit(NemeioTrigger.WantFactoryReset, NemeioState.FactoryReset)
                .OnEntryAsync(EntryReadyAsync)
                .OnExitAsync(ExitReadyAsync);

            _stateMachine.Configure(NemeioState.FactoryReset)
                .SubstateOf(NemeioState.Connected)
                .Ignore(NemeioTrigger.RefreshKeepAlive)
                .Ignore(NemeioTrigger.RefreshBattery)
                .Ignore(NemeioTrigger.Initialize)
                .Ignore(NemeioTrigger.KeyboardInitialized)
                .Ignore(NemeioTrigger.StartSync)
                .Ignore(NemeioTrigger.LayoutSynced)
                .InternalTransitionAsync(NemeioTrigger.StartFactoryReset, FactoryResetAsync)
                .Permit(NemeioTrigger.KeyboardUnplugged, NemeioState.Disconnecting)
                .Permit(NemeioTrigger.CancelFactoryReset, NemeioState.Ready)
                .Permit(NemeioTrigger.EndFactoryReset, NemeioState.Sync);

            _stateMachine.Configure(NemeioState.Disconnecting)
                .Ignore(NemeioTrigger.RefreshKeepAlive)
                .Ignore(NemeioTrigger.RefreshBattery)
                .Ignore(NemeioTrigger.Initialize)
                .Ignore(NemeioTrigger.KeyboardInitialized)
                .Ignore(NemeioTrigger.StartSync)
                .Ignore(NemeioTrigger.LayoutSynced)
                .Ignore(NemeioTrigger.KeyboardUnplugged)
                .Ignore(NemeioTrigger.WantFactoryReset)
                .Ignore(NemeioTrigger.StartFactoryReset)
                .Ignore(NemeioTrigger.EndFactoryReset)
                .Ignore(NemeioTrigger.CancelFactoryReset);
        }

        public async Task StartSynchronizationAsync()
        {
            _logger.LogTrace($"Synchronization start on Nemeio <{Identifier}>");

            await _stateMachine.FireAsync(NemeioTrigger.StartSync);
        }

        public async Task EndSynchronizationAsync()
        {
            _logger.LogTrace($"Synchronization end on Nemeio <{Identifier}>");

            await _stateMachine.FireAsync(NemeioTrigger.LayoutSynced);
        }

        protected override async Task InitKeyboardAsync()
        {
            await Task.Yield();

            //  Workaround until we add real way to know screen type
            Screen = Versions.ScreenType == ScreenType.Holitech
                ? _screenFactory.CreateHolitechScreen()
                : _screenFactory.CreateEinkScreen();

            FetchBattery();
            FetchLayoutHashed();
            LogLayouts();
        }

        #region Ready

        private async Task EntryReadyAsync()
        {
            await Task.Yield();

            _keepAliveTimer.OnTimeElapsed += KeepAliveTimer_Elapsed;
            _keepAliveTimer.Start();

            _batteryTimer.OnTimeElapsed += BatteryTimer_Elapsed;
            _batteryTimer.Start();
        }

        private async Task ExitReadyAsync()
        {
            await Task.Yield();

            _keepAliveTimer.Stop();
            _batteryTimer.Stop();
        }

        private async void KeepAliveTimer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                await _stateMachine.FireAsync(NemeioTrigger.RefreshKeepAlive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "KeepAliveTimer_Elapsed");
            }
        }
        private async void BatteryTimer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                await _stateMachine.FireAsync(NemeioTrigger.RefreshBattery);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "BatteryTimer_Elapsed");
            }
        }

        #endregion

        private async Task RefreshKeepAliveAsync()
        {
            await Task.Yield();

            try
            {
                _keepAliveMonitor.Ping();
            }
            catch (CommunicationTimeoutException)
            {
                _logger.LogWarning($"No response to keepAlive ping");
            }
            catch (KeyboardException exception)
            {
                _logger.LogError(exception, "RefreshKeepAliveAsync failed");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "RefreshKeepAliveAsync failed");
            }
        }

        private async Task RefreshBatteryAsync()
        {
            await Task.Yield();

            try
            {
                FetchBattery();
            }
            catch (FetchBatteryFailedException exception)
            {
                _logger.LogError(exception, "RefreshBatteryAsync failed");
            }
        }

        #region Events

        private void ConfigurationChangedMonitor_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            SelectedLayoutId = e.Configuration;

            OnSelectedLayoutChanged?.Invoke(this, EventArgs.Empty);
        }

        private void KeyPressedMonitor_OnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            _logger.LogTrace($"<{e.Keystrokes.Count()}> key(s) has been pressed");

            var keystrokes = Screen.TransformKeystrokes(e.Keystrokes);

            OnKeyPressed?.Invoke(this, new KeyboardKeyPressedEventArgs(SelectedLayoutId, keystrokes));
        }

        #endregion

        #region Layout Hashes

        /// <summary>
        /// Allow to fetch keyboard's layout hashes
        /// </summary>
        /// <exception cref="FetchLayoutHashFailedException">Throw if can't retrieve layouts from keyboard</exception>
        private void FetchLayoutHashed()
        {
            try
            {
                ExecuteWithRetry("Get layout hashes", () =>
                {
                    LayoutIdWithHashs = _getLayoutMonitor.AskLayoutIds().ToList();
                });
            }
            catch (KeyboardException exception)
            {
                var message = $"Failed to fetch layout hashes from keyboard <{Identifier}>";

                _logger.LogError(exception, message);

                throw new FetchLayoutHashFailedException(message, exception);
            }
        }

        #endregion

        private void LogLayouts()
        {
            _logger.LogTrace($"Nemeio <{Identifier}> layouts:");

            foreach (var idWithHash in LayoutIdWithHashs)
            {
                _logger.LogTrace($"* id '{idWithHash.Id}' hash '{idWithHash.Hash}'");
            }
        }
    }
}
