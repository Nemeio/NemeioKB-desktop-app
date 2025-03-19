using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.Builds;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Exceptions;
using Nemeio.Core.Keyboard.CommunicationMode;
using Nemeio.Core.Keyboard.Errors;
using Nemeio.Core.Keyboard.FactoryReset;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Parameters;
using Nemeio.Core.Keyboard.SerialNumber;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Keyboard.Version;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Services.Batteries;
using Nemeio.Core.Tools.Retry;
using Nemeio.Core.Tools.Stoppable;
using Stateless;

namespace Nemeio.Core.Keyboard
{
    public abstract class Nemeio : AsyncStoppable, INemeio
    {
        public enum NemeioTrigger
        {
            Initialize,
            KeyboardInitialized,
            StartSync,
            LayoutSynced,
            KeyboardUnplugged,
            RefreshKeepAlive,
            RefreshBattery,
            WantFactoryReset,
            StartFactoryReset,
            EndFactoryReset,
            CancelFactoryReset
        }

        public const string EmptyName = "<???>";
        private const uint DefaultRetryCount = 2;

        protected readonly ILogger _logger;
        protected readonly IKeyboardCommandExecutor _commandExecutor;
        protected readonly IRetryHandler _retryHandler;
        protected readonly CancellationTokenSource _cancelToken;

        private readonly IKeyboardCrashLogger _crashLogger;

        private readonly IBatteryMonitor _batteryMonitor;
        private readonly ISerialNumberMonitor _serialNumberMonitor;
        private readonly IVersionMonitor _versionMonitor;
        private readonly IKeyboardFailuresMonitor _keyboardFailuresMonitor;
        private readonly ICommunicationModeMonitor _communicationModeMonitor;
        private readonly IParametersMonitor _parametersMonitor;
        private readonly IFactoryResetMonitor _factoryResetMonitor;

        protected StateMachine<NemeioState, NemeioTrigger> _stateMachine;

        public string Identifier { get; private set; }
        public CommunicationType CommunicationType { get; private set; }
        public FirmwareVersions Versions { get; protected set; }
        public NemeioState State { get; private set; }
        public BatteryInformation Battery { get; private set; }
        public NemeioSerialNumber SerialNumber { get; private set; }
        public KeyboardParameters Parameters { get; private set; }
        public string Name { get; private set; }
        public IList<KeyboardFailure> Failures { get; private set; }
        public System.Version ProtocolVersion { get; private set; }

        public event EventHandler<StateChangedEventArgs> OnStateChanged;
        public event EventHandler OnBatteryLevelChanged;
        private readonly List<NemeioState> StatesHistory = new List<NemeioState>();
        private readonly List<NemeioState> RequiredStates = new List<NemeioState>() { NemeioState.Sync, NemeioState.Ready, NemeioState.Init };
        
        public bool SyncedReady { get { return RequiredStates.All(StatesHistory.Contains); } }

        public Nemeio(ILoggerFactory loggerFactory, string identifier, System.Version protocolVersion, CommunicationType type, IKeyboardCommandExecutor commandExecutor, IMonitorFactory monitorFactory, IKeyboardCrashLogger crashLogger, IRetryHandler retryHandler)
            : base(false)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            ProtocolVersion = protocolVersion ?? throw new ArgumentNullException(nameof(protocolVersion));
            CommunicationType = type;

            _logger = loggerFactory.CreateLogger<Nemeio>();

            _cancelToken = new CancellationTokenSource();
            _retryHandler = retryHandler ?? throw new ArgumentNullException(nameof(retryHandler));

            _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
            _commandExecutor.OnStopRaised += CommandExecutor_OnStopRaised;

            _crashLogger = crashLogger ?? throw new ArgumentNullException(nameof(crashLogger));

            _batteryMonitor = monitorFactory.CreateBatteryMonitor(_commandExecutor);
            _serialNumberMonitor = monitorFactory.CreateSerialNumberMonitor(_commandExecutor);
            _versionMonitor = monitorFactory.CreateVersionMonitor(_commandExecutor);
            _keyboardFailuresMonitor = monitorFactory.CreateKeyboardFailuresMonitor(_commandExecutor);
            _parametersMonitor = monitorFactory.CreateParametersMonitor(ProtocolVersion, _commandExecutor);
            _communicationModeMonitor = monitorFactory.CreateCommunicationModeMonitor(_commandExecutor);
            _factoryResetMonitor = monitorFactory.CreateFactoryResetMonitor(commandExecutor);

            _stateMachine = new StateMachine<NemeioState, NemeioTrigger>(NemeioState.Connected);
            _stateMachine.OnTransitioned((stateMachineAction) =>
            {
                _logger.LogInformation($"Transition completed: <{stateMachineAction.Destination}> from trigger <{stateMachineAction.Trigger}>");

                State = stateMachineAction.Destination;
                StatesHistory.Add(State);
                OnStateChanged?.Invoke(this, new StateChangedEventArgs(State, this));
            });

            _logger.LogInformation($"Nemeio with identifier <{Identifier}> use communication protocol <{ProtocolVersion}>");
        }

        public override async Task StopAsync()
        {
            if (Started)
            {
                _logger.LogInformation($"Ask stop on Nemeio <{Identifier}>");

                _cancelToken.Cancel();

                await _commandExecutor.StopAsync();
            }

            await base.StopAsync();
        }

        #region Initialize Keyboard

        public async Task InitializeAsync()
        {
            try
            {
                await _commandExecutor.Initialize();

                await _stateMachine.FireAsync(NemeioTrigger.Initialize);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Initialize failed");

                await StopAsync();

                throw;
            }
        }

        /// <summary>
        /// Called by state machine when keyboard initialize.
        /// Already have a try...catch which call stop when an error occur.
        /// Will automatically call trigger "KeyboardInitialized" when finish with success.
        /// </summary>
        protected abstract Task InitKeyboardAsync();

        protected async Task InitOnEntryAsync()
        {
            _logger.LogInformation($"Init keyboard with <{Identifier}> of type <{GetType().Name}>");

            try
            {
                AliveState = AliveState.Starting;

                await Task.Run(async () =>
                {
                    FetchVersion();
                    FetchSerialNumber();
                    GenerateNemeioName();
                    FetchParameters();
                    FetchCrashes();

                    await InitKeyboardAsync();

                    AliveState = AliveState.Started;

                    await _stateMachine.FireAsync(NemeioTrigger.KeyboardInitialized);

                }, _cancelToken.Token);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "InitOnEntryAsync");

                await StopAsync();

                throw new InitializationFailedException(Identifier);
            }
        }

        #endregion

        #region Disconnect Keyboard

        public async Task DisconnectAsync()
        {
            await _stateMachine.FireAsync(NemeioTrigger.KeyboardUnplugged);
        }

        #endregion

        #region Name

        protected void GenerateNemeioName()
        {
            var serialString = SerialNumber.ToString().Replace("-", "");
            Name = serialString.Substring(serialString.Length - 4).ToUpper();
        }

        #endregion

        #region Battery

        /// <summary>
        /// Allow to fetch keyboard's battery level.
        /// We will try 3 times to fetch parameters before throw exception.
        /// </summary>
        /// <exception cref="FetchBatteryFailedException">Throw if can't retrieve battery level from keyboard</exception>
        protected void FetchBattery()
        {
            try
            {
                ExecuteWithRetry("Get battery", () =>
                {
                    Battery = _batteryMonitor.AskBattery();
                    OnBatteryLevelChanged?.Invoke(this, EventArgs.Empty);
                });
            }
            catch (KeyboardException exception)
            {
                var message = $"Fail fetch battery level from keyboard <{Identifier}>";

                _logger.LogError(exception, message, exception);

                throw new FetchBatteryFailedException(message, exception);
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to fetch battery level from keyboard <{Identifier}>";

                _logger.LogError(communicationException, message);

                throw new FetchBatteryFailedException(message, communicationException);
            }
        }

        #endregion

        #region Parameters

        /// <summary>
        /// Allow to fetch keyboard's parameters.
        /// We will try 3 times to fetch parameters before throw exception.
        /// </summary>
        /// <exception cref="FetchParametersFailedException">Throw if can't retrieve parameters from keyboard</exception>
        protected void FetchParameters()
        {
            try
            {
                ExecuteWithRetry("Get parameters", () =>
                {
                    Parameters = _parametersMonitor.GetParameters();
                });
            }
            catch (KeyboardException exception)
            {
                var message = $"Fail fetch parameters from keyboard <{Identifier}>";

                _logger.LogError(exception, message, exception);

                throw new FetchParametersFailedException(message, exception);
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to fetch parameters from keyboard <{Identifier}>";

                _logger.LogError(communicationException, message);

                throw new FetchParametersFailedException(message, communicationException);
            }
        }

        /// <summary>
        /// Allow to update keyboard's parameters.
        /// </summary>
        /// <exception cref="SetParametersFailedException">Throw if can't update keyboard's parameters</exception>
        public async Task UpdateParametersAsync(KeyboardParameters parameters)
        {
            await Task.Yield();

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            try
            {
                _parametersMonitor.SetParameters(parameters);

                Parameters = parameters;
            }
            catch (KeyboardException exception)
            {
                var message = $"Set keyboard parameters failed on keyboard <{Identifier}>";

                _logger.LogError(exception, message);

                throw new SetParametersFailedException(message, exception);
            }
        }

        public async Task RefreshParametersAsync()
        {
            await Task.Yield();

            FetchParameters();
        }

        #endregion

        #region Crash

        /// <summary>
        /// Allow to fetch keyboard's failures.
        /// We will try 3 times to fetch parameters before throw exception.
        /// </summary>
        /// <exception cref="GetKeyboardFailuresFailedException">Throw if can't retrieve failures from keyboard</exception>
        protected void FetchCrashes()
        {
            try
            {
                ExecuteWithRetry("Get crashes", () =>
                {
                    var failures = _keyboardFailuresMonitor.AskKeyboardFailures();
                    if (failures.Any())
                    {
                        Failures = failures;

                        _crashLogger.WriteKeyboardCrashLog(Versions, failures);
                    }
                });
            }
            catch (KeyboardException exception)
            {
                var message = $"Fail fetch crash from keyboard <{Identifier}>";

                _logger.LogError(exception, message, exception);

                throw new GetKeyboardFailuresFailedException(message, exception);
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to fetch crash from keyboard <{Identifier}>";

                _logger.LogError(communicationException, message);

                throw new GetKeyboardFailuresFailedException(message, communicationException);
            }
        }

        #endregion

        #region Versions

        /// <summary>
        /// Allow to fetch keyboard's versions.
        /// We will try 3 times to fetch parameters before throw exception.
        /// </summary>
        /// <exception cref="FetchVersionsFailedException">Throw if can't retrieve version from keyboard</exception>
        protected void FetchVersion()
        {
            try
            {
                ExecuteWithRetry("Get Keyboard Version", () =>
                {
                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    Versions = _versionMonitor.AskVersions();

                    stopWatch.Stop();

                    _logger.LogInformation($"Keyboard <{Identifier}> has versions <{Versions}> in <{stopWatch.ElapsedMilliseconds}> milliseconds");
                });
            }
            catch (KeyboardException exception)
            {
                var message = $"Fail fetch versions from keyboard <{Identifier}>";

                _logger.LogError(exception, message, exception);

                throw new FetchVersionsFailedException(message, exception);
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to fetch versions from keyboard <{Identifier}>";

                _logger.LogError(communicationException, message);

                throw new FetchVersionsFailedException(message, communicationException);
            }
        }

        #endregion

        #region Serial Number

        /// <summary>
        /// Allow to fetch keyboard's battery level.
        /// We will try 3 times to fetch serial number before throw exception.
        /// </summary>
        /// <exception cref="FetchSerialNumberFailedException">Throw if can't retrieve serial number from keyboard</exception>
        protected void FetchSerialNumber()
        {
            try
            {
                ExecuteWithRetry("Get Serial number", () =>
                {
                    SerialNumber = _serialNumberMonitor.AskSerialNumber();

                    _logger.LogInformation($"Keyboard with <{Identifier}> have serial number <{SerialNumber}>");
                });
            }
            catch (KeyboardException exception)
            {
                var message = $"Fail fetch serial number from keyboard <{Identifier}>";

                _logger.LogError(exception, message, exception);

                throw new FetchSerialNumberFailedException(message, exception);
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to fetch serial number from keyboard <{Identifier}>";

                _logger.LogError(communicationException, message);

                throw new FetchSerialNumberFailedException(message, communicationException);
            }
        }

        #endregion

        #region Communication Mode

        /// <summary>
        /// Allow to set keyboard's communication mode.
        /// </summary>
        /// <exception cref="SetCommunicationModeFailedException">Throw if can't retrieve battery level from keyboard</exception>
        public Task SetHidModeAsync() => SetCommunicationMode(KeyboardCommunicationMode.Hid);

        /// <summary>
        /// Allow to set keyboard's communication mode.
        /// </summary>
        /// <exception cref="SetCommunicationModeFailedException">Throw if can't retrieve battery level from keyboard</exception>
        public Task SetAdvancedModeAsync() => SetCommunicationMode(KeyboardCommunicationMode.Advanced);

        /// <summary>
        /// Allow to set keyboard's communication mode.
        /// </summary>
        /// <exception cref="SetCommunicationModeFailedException">Throw if can't retrieve battery level from keyboard</exception>
        private async Task SetCommunicationMode(KeyboardCommunicationMode mode)
        {
            await Task.Yield();

            try
            {
                _communicationModeMonitor.SetCommunicationMode(mode);
            }
            catch (KeyboardException exception)
            {
                var message = $"Failed to set communication mode to <{mode}> for keyboard <{Identifier}>";

                _logger.LogError(exception, message);

                throw new SetCommunicationModeFailedException(message, exception);
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to set communication mode to <{mode}> for keyboard <{Identifier}> because of communication error";

                _logger.LogError(communicationException, message);

                throw new SetCommunicationModeFailedException(message, communicationException);
            }
        }

        #endregion

        #region Factory Reset

        public async Task WantFactoryResetAsync()
        {
            _logger.LogInformation($"User want factory reset for keyboard <{Identifier}>");

            await _stateMachine.FireAsync(NemeioTrigger.WantFactoryReset);
        }

        public async Task ConfirmFactoryResetAsync()
        {
            _logger.LogInformation($"User confirm factory reset for keyboard <{Identifier}>");

            await _stateMachine.FireAsync(NemeioTrigger.StartFactoryReset);
        }

        public async Task CancelFactoryResetAsync()
        {
            _logger.LogInformation($"User cancel factory reset for keyboard <{Identifier}>");

            await _stateMachine.FireAsync(NemeioTrigger.CancelFactoryReset);
        }

        protected async Task FactoryResetAsync()
        {
            try
            {
                _logger.LogInformation($"Ask Factory reset on keyboard <{Identifier}>");

                _factoryResetMonitor.AskFactoryReset();

                await _stateMachine.FireAsync(NemeioTrigger.EndFactoryReset);
            }
            catch (KeyboardException exception)
            {
                _logger.LogError(exception, $"Failed to factory reset on Nemeio <{Identifier}>");

                throw new FactoryResetFailedException(this);
            }
            catch (KeyboardCommunicationException communicationException)
            {
                //  Something goes wrong when try to communicate with Nemeio

                var message = $"Impossible to factory reset on Nemeio <{Identifier}> because of communication error";

                _logger.LogError(communicationException, message);

                throw new FactoryResetFailedException(this);
            }
        }

        #endregion

        #region Tools

        protected void ExecuteWithRetry(string actionName, Action executableAction, uint nbRetry = DefaultRetryCount)
        {
            try
            {
                var retryAction = new SyncRetryAction(actionName, nbRetry, executableAction);

                _retryHandler.Execute(retryAction);
            }
            catch (RetryFailedException exception)
            {
                _logger.LogError(exception, $"Retry failed");

                //  By default we want to manage original exception
                throw exception.InnerException;
            }
        }

        #endregion

        private async void CommandExecutor_OnStopRaised(object sender, EventArgs e)
        {
            //  KeyboardCommandExecutor is a primary dependency
            //  if it stop we must stop too

            if (Started)
            {
                _logger.LogInformation($"Keyboard <{Identifier}>'s command executor stopped, we stop too...");

                await StopAsync();
            }
        }
    }
}
