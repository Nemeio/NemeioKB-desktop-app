using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Configurations;
using Nemeio.Core.Keyboard.Configurations.Add;
using Nemeio.Core.Keyboard.Configurations.Delete;
using Nemeio.Core.Keyboard.Connection;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Layouts.Synchronization.Contexts;
using Nemeio.Core.Layouts.Synchronization.Contexts.State;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Stateless;

namespace Nemeio.Core.Layouts.Synchronization
{
    public class Synchronizer : ISynchronizer
    {
        private sealed class SynchronizableNemeioProxy : KeyboardProxy, ISynchronizableNemeioProxy
        {
            private readonly IConfigurationHolder _configurationHolder;
            private readonly IAddConfigurationHolder _addConfigurationHolder;
            private readonly IDeleteConfigurationHolder _deleteConfigurationHolder;

            public IList<LayoutIdWithHash> LayoutIdWithHashs=> _configurationHolder.LayoutIdWithHashs;
            public LayoutId SelectedLayoutId => _configurationHolder.SelectedLayoutId;
            public IScreen Screen => _configurationHolder.Screen;

            public event EventHandler OnSelectedLayoutChanged;

            public SynchronizableNemeioProxy(Keyboard.Nemeios.INemeio nemeio)
                : base(nemeio)
            {
                _configurationHolder = nemeio as IConfigurationHolder;
                _addConfigurationHolder = nemeio as IAddConfigurationHolder;
                _deleteConfigurationHolder = nemeio as IDeleteConfigurationHolder;
            }

            public Task AddLayoutAsync(ILayout layout) => _addConfigurationHolder.AddLayoutAsync(layout);
            public Task DeleteLayoutAsync(LayoutId layoutId) => _deleteConfigurationHolder.DeleteLayoutAsync(layoutId);
            public Task StartSynchronizationAsync() => _configurationHolder.StartSynchronizationAsync();
            public Task EndSynchronizationAsync() => _configurationHolder.EndSynchronizationAsync();
        }

        public enum SynchornizerState
        {
            Waiting,
            Syncing
        }

        enum SynchronizerTrigger
        {
            Synchronize,
            Synchronized
        }

        private readonly ILogger _logger;
        private readonly Stopwatch _stopWatcher;
        private readonly ISystem _system;
        private readonly IDictionary<string, ISynchronizationContext> _keyboardContexts;

        private ISynchronizationContext _systemContext;

        private readonly ISynchronizationContextFactory _contextFactory;
        private readonly IKeyboardController _keyboardController;
        private readonly IScreenFactory _screenFactory;

        private readonly StateMachine<SynchornizerState, SynchronizerTrigger> _stateMachine;
        private readonly SemaphoreSlim _synchronizationSephamore = new SemaphoreSlim(1, 1);

        private SynchronizableNemeioProxy _proxy;

        public event EventHandler OnStateChanged;

        public SynchornizerState State { get; private set; }

        public Synchronizer(ILoggerFactory loggerFactory, ISystem system, ISynchronizationContextFactory contextFactory, IKeyboardController keyboardController, IScreenFactory screenFactory)
        {
            _logger = loggerFactory.CreateLogger<Synchronizer>();
            _stopWatcher = new Stopwatch();
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _system.OnLayoutsChanged += System_OnLayoutsChanged;

            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));

            _keyboardContexts = new Dictionary<string, ISynchronizationContext>();

            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _keyboardController.OnKeyboardConnected += KeyboardController_OnKeyboardConnected;
            _keyboardController.OnKeyboardDisconnecting += KeyboardController_OnKeyboardDisconnecting;

            _screenFactory = screenFactory ?? throw new ArgumentNullException(nameof(screenFactory));

            _stateMachine = new StateMachine<SynchornizerState, SynchronizerTrigger>(SynchornizerState.Waiting);
            _stateMachine.OnTransitioned((stateMachineAction) => 
            {
                _logger.LogInformation($"[Synchronizer] Transitioned: <{stateMachineAction.Destination}> from trigger <{stateMachineAction.Trigger}>");

                State = stateMachineAction.Destination;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
            });

            _stateMachine.Configure(SynchornizerState.Waiting)
                .Permit(SynchronizerTrigger.Synchronize, SynchornizerState.Syncing)
                .Ignore(SynchronizerTrigger.Synchronized);

            _stateMachine.Configure(SynchornizerState.Syncing)
                .OnEntryAsync(StartSynchronizationAsync)
                .Permit(SynchronizerTrigger.Synchronized, SynchornizerState.Waiting)
                .Ignore(SynchronizerTrigger.Synchronize);

            Initialization();
        }

        public async Task SynchronizeAsync() => await _stateMachine.FireAsync(SynchronizerTrigger.Synchronize);

        public ISynchronizationContextState GetSynchronizationStateFor(Keyboard.Nemeios.INemeio nemeio)
        {
            if (nemeio == null)
            {
                throw new ArgumentNullException(nameof(nemeio));
            }

            ISynchronizationContextState contextState = null;

            if (_keyboardContexts.ContainsKey(nemeio.Identifier))
            {
                var syncContext = _keyboardContexts[nemeio.Identifier];
                if (syncContext is ISynchronizationStateHolder stateHolder)
                {
                    contextState = stateHolder.State;
                }
            }

            return contextState;
        }

        private void Initialization()
        {
            Task.Run(async () =>
            {
                try
                {
                    CheckRunningKeyboard();

                    _logger.LogInformation($"Start sync from Initialization");

                    //  By default we create layout with eink screen
                    var screen = _screenFactory.CreateEinkScreen();

                    _systemContext = _contextFactory.CreateSystemSynchronizationContext(screen);
                    _systemContext.NeedSynchronization();

                    await StartSynchronizeAsync();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Initialization failed!");
                }
            });
        }

        private async Task StartSynchronizeAsync()
        {
            await _stateMachine.FireAsync(SynchronizerTrigger.Synchronize);
        }

        private void CheckRunningKeyboard()
        {
            if (_proxy != null)
            {
                RegisterKeyboardEventIfNeeded(_proxy);

                CheckSyncState(_proxy);
            }
        }

        private void CheckSyncState(SynchronizableNemeioProxy proxy)
        {
            if (proxy.State == NemeioState.Sync)
            {
                _logger.LogTrace($"Keyboard with identifier <{proxy.Identifier}> change state to sync");

                if (!_keyboardContexts.ContainsKey(proxy.Identifier))
                {
                    var keyboardContext = _contextFactory.CreateKeyboardSynchronizationContext(proxy);

                    _keyboardContexts.Add(proxy.Identifier, keyboardContext);
                }
            }
        }

        private void RegisterKeyboardEventIfNeeded(SynchronizableNemeioProxy proxy)
        {
            if (proxy == null)
            {
                throw new ArgumentNullException(nameof(proxy));
            }

            if (!_keyboardContexts.ContainsKey(proxy.Identifier))
            {
                proxy.OnStateChanged += Proxy_OnStateChanged;
            }
        }

        private async Task StartSynchronizationAsync()
        {
            await _synchronizationSephamore.WaitAsync();

            await Task.Run(async () => 
            {
                try
                {
                    _stopWatcher.Reset();
                    _stopWatcher.Start();

                    _logger.LogTrace($"Start sync layouts on Thread <{Thread.CurrentThread.ManagedThreadId}>");

                    if (_systemContext != null)
                    {
                        await _systemContext.SyncIfNeededAsync();
                    }

                    if (_keyboardContexts.Any())
                    {
                        foreach (var context in _keyboardContexts.Values.ToList())
                        {
                            await context.SyncAsync();
                        }
                    }

                    _logger.LogTrace("End sync layouts");

                    _stopWatcher.Stop();

                    _logger.LogTrace($"Synchronization time elapsed <{_stopWatcher.ElapsedMilliseconds}> milliseconds");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Synchronization failed");
                }
                finally
                {
                    _synchronizationSephamore.Release();

                    await _stateMachine.FireAsync(SynchronizerTrigger.Synchronized);
                }
            });
        }

        #region Events

        private void KeyboardController_OnKeyboardConnected(object sender, KeyboardStatusChangedEventArgs e)
        {
            var proxy = KeyboardProxy.CastTo<SynchronizableNemeioProxy>(e.Keyboard);
            if (proxy != null)
            {
                _proxy = proxy;

                RegisterKeyboardEventIfNeeded(_proxy);
            }
        }

        private void KeyboardController_OnKeyboardDisconnecting(object sender, KeyboardStatusChangedEventArgs e)
        {
            if (_proxy != null && _proxy.Is(e.Keyboard))
            {
                _proxy.OnStateChanged -= Proxy_OnStateChanged;

                if (_keyboardContexts.ContainsKey(_proxy.Identifier))
                {
                    _keyboardContexts.Remove(_proxy.Identifier);
                }
            }
        }

        private async void Proxy_OnStateChanged(object sender, StateChangedEventArgs e)
        {
            if (_proxy != null && _proxy.Is(e.Nemeio))
            {
                CheckSyncState(_proxy);

                if (_proxy.State == NemeioState.Sync && _stateMachine.State == SynchornizerState.Waiting)
                {
                    _logger.LogInformation($"Start sync from Proxy_OnStateChanged");

                    if (_systemContext != null)
                    {
                        _systemContext = _contextFactory.CreateSystemSynchronizationContext(_proxy.Screen);
                        _systemContext.NeedSynchronization();
                    }

                    await StartSynchronizeAsync();
                }
            }
        }

        private async void System_OnLayoutsChanged(object sender, EventArgs e)
        {
            _logger.LogInformation($"Start sync from System_OnLayoutsChanged");

            _systemContext?.NeedSynchronization();

            await StartSynchronizeAsync();
        }

        #endregion
    }
}
