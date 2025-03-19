using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Layouts.Active.Exceptions;
using Nemeio.Core.Layouts.Active.Requests.Base;
using Nemeio.Core.Layouts.Active.Requests.Factories;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Systems.Sessions;
using Nemeio.Core.Tools.Stoppable;

namespace Nemeio.Core.Layouts.Active
{
    public sealed class ActiveLayoutChangeHandler : AsyncStoppable, IActiveLayoutChangeHandler
    {
        private readonly ILogger _logger;
        private readonly ISystem _system;
        private readonly IChangeRequestFactory _changeRequestFactory;
        private readonly BlockingCollection<IChangeRequest> _requests;
        private readonly CancellationTokenSource _cancellation;

        public IActiveLayoutSynchronizer Synchronizer { get; private set; }

        public ActiveLayoutChangeHandler(ILoggerFactory loggerFactory, ISystem system, IChangeRequestFactory changeRequestFactory, IActiveLayoutSynchronizer synchronizer)
            : base(true)
        {
            _logger = loggerFactory.CreateLogger<ActiveLayoutChangeHandler>();
            _system = system ?? throw new ArgumentNullException(nameof(system));
            _changeRequestFactory = changeRequestFactory ?? throw new ArgumentNullException(nameof(changeRequestFactory));
            _requests = new BlockingCollection<IChangeRequest>();
            _cancellation = new CancellationTokenSource();

            Synchronizer = synchronizer ?? throw new ArgumentNullException(nameof(synchronizer));
            StartPolling();
        }

        public override void Stop()
        {
            base.Stop();

            _cancellation.Cancel();
        }

        public async Task RequestResetHistoricAsync()
        {
            await Task.Yield();

            Synchronizer.ResetHistoric();
        }

        public async Task RequestApplicationShutdownAsync(INemeio nemeio)
        {
            var request = _changeRequestFactory.CreateApplicationShutdownRequest(nemeio);

            await PostRequestAsync(request);
        }

        public async Task RequestForegroundApplicationChangeAsync(INemeio nemeio, Application application)
        {
            var request = _changeRequestFactory.CreateForegroundApplicationRequest(nemeio, application);

            await PostRequestAsync(request);
        }

        public async Task RequestHidSystemChangeAsync(INemeio nemeio)
        {
            if (Synchronizer.Syncing)
            {
                _logger.LogWarning($"Request on <System_OnSelectedLayoutChanged> bypassed because we syncing");

                return;
            }

            var request = _changeRequestFactory.CreateHidSystemRequest(nemeio);

            await PostRequestAsync(request);
        }

        public async Task RequestHistoricChangeAsync(INemeio nemeio, bool isBack)
        {
            var request = _changeRequestFactory.CreateHistoricRequest(nemeio, isBack);

            await PostRequestAsync(request);
        }

        public async Task RequestKeyboardSelectionChangeAsync(INemeio nemeio)
        {
            if (Synchronizer.Syncing)
            {
                _logger.LogWarning($"Request on <Runner_OnSelectedLayoutChanged> bypassed because we syncing");

                return;
            }

            var request = _changeRequestFactory.CreateKeyboardSelectionRequest(nemeio);

            await PostRequestAsync(request);
        }

        public async Task RequestKeyPressChangeAsync(INemeio nemeio, LayoutId id)
        {
            var request = _changeRequestFactory.CreateKeyPressRequest(nemeio, id);

            await PostRequestAsync(request);
        }

        public async Task RequestMenuChangeAsync(INemeio nemeio, LayoutId id)
        {
            var request = _changeRequestFactory.CreateMenuRequest(nemeio, id);

            await PostRequestAsync(request);
        }

        public async Task RequestSessionChangeAsync(INemeio nemeio, SessionState state)
        {
            var request = _changeRequestFactory.CreateSessionRequest(nemeio, state);

            await PostRequestAsync(request);
        }

        private async Task PostRequestAsync(IChangeRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //  We never add request when session is lock
            if (_system.SessionState == SessionState.Lock)
            {
                _logger.LogInformation($"Bypass add change active layout request because session is closed");

                return;
            }

            _logger.LogInformation($"Add request <{request.GetType().Name}> on queue");

            _requests.Add(request);

            try
            {
                await request.ExecuteAsync();
            }
            catch (Exception exception)
            {
                throw new ActiveLayoutRequestException($"Execute request failed", exception);
            }
        }
           
        private void StartPolling()
        {
            Task.Run(async () => 
            { 
                try
                {
                    while (!_cancellation.IsCancellationRequested)
                    {
                        var request = _requests.Take(_cancellation.Token);

                        try
                        {
                            await Synchronizer.PostRequestAsync(request);
                        }
                        catch (ActiveLayoutSynchronizationFailed exception)
                        {
                            _logger.LogError(exception, "Sync failed");
                        }
                        finally
                        {
                            await CheckAndResyncIfNeeded();
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Run stopped because of an error");
                }
                finally
                {
                    _logger.LogWarning($"Polling task ended for ActiveLayoutChangeHandler");
                }
            });
        }

        private async Task CheckAndResyncIfNeeded()
        {
            //  This is the only time we consider keyboard as master
            //  Because of custom layout

            //  If keyboard's current layout is not equals to current system layout
            //  We resync

            /*ILayoutHolderNemeioProxy proxy = null;

            if (_keyboardController.Connected)
            {
                proxy = KeyboardProxy.CastTo<LayoutHolderNemeioProxy>(_keyboardController.Nemeio);
            }

            if (_proxy != null && _proxy.SelectedLayoutHash != null && Synchronizer.LastSynchronizedLayout != null && !Synchronizer.LastSynchronizedLayout.Hash.Equals(_proxy.SelectedLayoutHash))
            {
                //  We not use method 'RequestKeyboardSelectionChangeAsync' to avoid
                //  blocking loop

                var request = _changeRequestFactory.CreateKeyboardSelectionRequest();

                await Synchronizer.PostRequestAsync(_proxy, request);
            }*/
        }
    }
}
