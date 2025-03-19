using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Builds;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Watchers;
using Nemeio.Core.Keyboard.Connection;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Nemeios.Runner;
using Nemeio.Core.Keyboard.Sessions;
using Nemeio.Core.Keyboard.Sessions.Strategies;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Sessions;
using Nemeio.Core.Systems.Sleep;

namespace Nemeio.Core.Keyboard
{
    public class KeyboardController : IKeyboardController
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IKeyboardProvider _keyboardProvider;
        private readonly INemeioBuilder _nemeioBuilder;
        private readonly ISystem _system;
        private readonly SemaphoreSlim _startConnectionSemaphore;
        private readonly SemaphoreSlim _systemStateChangedSemaphore;
        private readonly INemeioLayoutEventStrategyFactory _keyboardSessionEventStrategyFactory;

        private INemeioLayoutEventStrategy _sessionStrategy;
        private readonly List<PackageUpdateState> PackageUpdaterStateHistory = new List<PackageUpdateState>();
        private readonly List<PackageUpdateState> RequiredPackageUpdaterStates = new List<PackageUpdateState>() { PackageUpdateState.Idle, PackageUpdateState.CheckInternetConnection};
        private bool PackageUpdaterReady { get { return RequiredPackageUpdaterStates.All(PackageUpdaterStateHistory.Contains); } }

        public event EventHandler<KeyboardStatusChangedEventArgs> OnKeyboardConnected;
        public event EventHandler<KeyboardStatusChangedEventArgs> OnKeyboardDisconnecting;
        public event EventHandler OnKeyboardDisconnected;
        public event EventHandler<KeyboardInitializationFailedEventArgs> OnKeyboardInitializationFailed;

        public INemeio Nemeio { get; set; }
        public bool Connected => Nemeio != null;

        public KeyboardController(ILoggerFactory loggerFactory, IKeyboardProvider keyboardProvider, INemeioBuilder nemeioBuilder, ISystem system, INemeioLayoutEventStrategyFactory keyboardSessionEventStrategyFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<KeyboardController>();
            _startConnectionSemaphore = new SemaphoreSlim(1, 1);
            _systemStateChangedSemaphore = new SemaphoreSlim(1, 1);

            _system = system ?? throw new ArgumentNullException(nameof(system));
            _system.OnSessionStateChanged += System_OnSessionStateChanged;
            _system.OnSleepModeChanged += System_OnSleepModeChanged;

            _nemeioBuilder = nemeioBuilder ?? throw new ArgumentNullException(nameof(nemeioBuilder));
            _keyboardProvider = keyboardProvider ?? throw new ArgumentNullException(nameof(keyboardProvider));
            _keyboardProvider.OnKeyboardConnected += KeyboardProvider_OnKeyboardConnected;
            _keyboardProvider.OnKeyboardDisconnected += KeyboardProvider_OnKeyboardDisconnected;

            _keyboardSessionEventStrategyFactory = keyboardSessionEventStrategyFactory ?? throw new ArgumentNullException(nameof(keyboardSessionEventStrategyFactory));
        }
        public Task RunAsync()
        {
            return CheckConnectedKeyboardsAsync();
        }

        private async Task CheckConnectedKeyboardsAsync(string lastKeyboardIdentifier = null, CommunicationType lastCommunicationType = CommunicationType.Serial)
        {
            if (_system.SessionState == SessionState.Lock)
            {
                _logger.LogInformation("Session is closed, we cannot check if a keyboard is connected");

                return;
            }

            await _startConnectionSemaphore.WaitAsync();

            try
            {
                var keyboard = _keyboardProvider.GetKeyboard((kb) => FilterKeyboard(kb, lastKeyboardIdentifier, lastCommunicationType));
                if (keyboard == null)
                {
                    _logger.LogInformation("No keyboard found. We wait for a keyboard");

                    return;
                }

                await StartNewNemeioAsync(keyboard);
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogError(exception, $"A keyboard is already connected");
            }
            finally
            {
                _startConnectionSemaphore.Release();
            }
        }

        private bool FilterKeyboard(Keyboard keyboard, string lastKeyboardIdentifier, CommunicationType lastCommunicationType)
        {
            if (keyboard == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(lastKeyboardIdentifier))
            {
                return true;
            }

            var isValid = !(keyboard.Identifier.Equals(lastKeyboardIdentifier) && keyboard.Communication == lastCommunicationType);

            return isValid;
        }

        private async Task StartNewNemeioAsync(Keyboard keyboard)
        {
            if (keyboard == null)
            {
                throw new ArgumentNullException(nameof(keyboard));
            }

            if (Nemeio != null)
            {
                throw new InvalidOperationException($"You can't start a new Nemeio <{keyboard.Identifier}> when another is already started <{Nemeio.Identifier}>");
            }

            try
            {
                var nemeio = await _nemeioBuilder.BuildAsync(keyboard);
                if (nemeio != null)
                {
                    Nemeio = nemeio;
                    Nemeio.OnStopRaised += Nemeio_OnStopRaised;
                    Nemeio.OnStateChanged += Nemeio_OnStateChanged;
                    _sessionStrategy = _keyboardSessionEventStrategyFactory.CreateStrategy(nemeio, _loggerFactory);

                    RaiseOnKeyboardConnected(Nemeio);

                    await Nemeio.InitializeAsync();

                    if (_sessionStrategy != null)
                    {
                        await _sessionStrategy.ConnectAsync();
                    }
                    _logger.LogInformation(Nemeio.Versions?.ToString());
                }
            }
            catch (InitializationFailedException exception)
            {
                _logger.LogError(exception, $"Nemeio init failed!");

                OnKeyboardInitializationFailed?.Invoke(this, new KeyboardInitializationFailedEventArgs(exception.Identifier));
            }
        }

        private void Nemeio_OnStateChanged(object sender, State.StateChangedEventArgs e)
        {
            CheckForegroundIfNeeded();
        }

        #region Events

        private void CheckForegroundIfNeeded()
        {
            var nemeioRunner = Nemeio as NemeioRunner;
            if (nemeioRunner != null && nemeioRunner.SyncedReady && PackageUpdaterReady)
            {
                _system.CheckForegroundApplication();
            }
        }
        private async void KeyboardProvider_OnKeyboardConnected(object sender, EventArgs e)
        {
            if (!Connected && _system.SessionState == SessionState.Open && _system.SleepMode == SleepMode.Awake)
            {
                await CheckConnectedKeyboardsAsync();
            }
        }

        private async void KeyboardProvider_OnKeyboardDisconnected(object sender, KeyboardDisconnectedEventArgs e)
        {
            if (Connected && Nemeio.Identifier.Equals(e.Identifier))
            {
                _logger.LogInformation($"Nemeio with identifier <{e.Identifier}> disconnected");

                await DisconnectNemeioAsync();
            }
        }

        private async void Nemeio_OnStopRaised(object sender, EventArgs e)
        {
            _logger.LogInformation($"Nemeio stopped, we disconnect everything");

            await DisconnectNemeioAsync();
        }

        private async void System_OnSessionStateChanged(object sender, EventArgs e)
        {
            await SystemStateChangedAsync();
        }

        private async void System_OnSleepModeChanged(object sender, EventArgs e)
        {
            await SystemStateChangedAsync();
        }

        private async Task SystemStateChangedAsync()
        {
            await _systemStateChangedSemaphore.WaitAsync();

            try
            {
                var systemIsInactive = _system.SessionState == SessionState.Lock || _system.SleepMode == SleepMode.Sleep;

                //  By default when session is unlocked and application doesn't have any
                //  connected keyboard, we try to find one
                if (!systemIsInactive && !Connected)
                {
                    _logger.LogInformation($"Session opening or system is awake, we don't have any keyboard, try to find one ...");

                    await CheckConnectedKeyboardsAsync();
                }
                //  Otherwise we stop the current connection
                else if (systemIsInactive && Connected)
                {
                    _logger.LogInformation($"Session closing or system sleep, we stop communication with Nemeio <{Nemeio?.Identifier}>");

                    if (_sessionStrategy != null)
                    {
                        await _sessionStrategy.SessionCloseAsync();
                    }

                    await DisconnectNemeioAsync(autoSearch: false);
                }
            }
            finally
            {
                _systemStateChangedSemaphore.Release();
            }
        }

        #endregion

        private async Task DisconnectNemeioAsync(bool autoSearch = true)
        {
            await _startConnectionSemaphore.WaitAsync();

            if (Nemeio == null)
            {
                //  Don't need to go further if Nemeio is already null

                return;
            }

            var currentNemeioIdentifier = Nemeio.Identifier;
            var currentNemeioCommunication = Nemeio.CommunicationType;

            RaiseOnKeyboardDisconnecting(Nemeio);

            try
            {
                if (_sessionStrategy != null)
                {
                    await _sessionStrategy.DisconnectAsync();
                }

                //  Current keyboard has been disconnected
                Nemeio.OnStateChanged -= Nemeio_OnStateChanged;
                Nemeio.OnStopRaised -= Nemeio_OnStopRaised;

                await Nemeio.StopAsync();
                await Nemeio.DisconnectAsync();

                Nemeio = null;
                _sessionStrategy = null;

                _logger.LogInformation($"Nemeio with identifier <{currentNemeioIdentifier}> has been closed");

                RaiseOnKeyboardDisconnected();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Disconnect Nemeio failed");
            }
            finally
            {
                _startConnectionSemaphore.Release();

                if (autoSearch)
                {
                    //  We automatically search the next one
                    await CheckConnectedKeyboardsAsync(currentNemeioIdentifier, currentNemeioCommunication);
                }
            }
        }
        private void RaiseOnKeyboardConnected(INemeio nemeio) => OnKeyboardConnected?.Invoke(this, new KeyboardStatusChangedEventArgs(nemeio));
        private void RaiseOnKeyboardDisconnecting(INemeio nemeio) => OnKeyboardDisconnecting?.Invoke(this, new KeyboardStatusChangedEventArgs(nemeio));
        private void RaiseOnKeyboardDisconnected() => OnKeyboardDisconnected?.Invoke(this, EventArgs.Empty);
        public void RaisePackageUpdaterStateChanged(PackageUpdateState state)
        {
            if (state == PackageUpdateState.CheckApplicationInstallation)
            {
                PackageUpdaterStateHistory.Clear();
            }
            PackageUpdaterStateHistory.Add(state);
            CheckForegroundIfNeeded();

        }
    }
}
