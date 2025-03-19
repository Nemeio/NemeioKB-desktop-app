using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Errors;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Tools.Stoppable;

namespace Nemeio.Core.Systems.Watchers
{
    /// <summary>
    /// Mechanism to harmonize the way system layout changes are tracked over various systems:
    /// basically the need is to hanlde here a strategy where we can decide to listen or not layout changes.
    /// Hence we can arbitrarily ignore some changes by means of a Start/Stop listening mechanism
    /// (This needs to be specifically imùplemented over the target system)
    /// </summary>
    public class SystemActiveLayoutWatcher : Stoppable, ISystemActiveLayoutWatcher
    {
        public const int ThreadPollingTimeout = 500;

        private readonly ILogger _logger;
        private readonly IErrorManager _errorManager;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ISystemActiveLayoutAdapter _adapter;
        private OsLayoutId _currentSystemLayout;

        public OsLayoutId CurrentSystemLayoutId => _currentSystemLayout;

        public event EventHandler OnSystemLayoutChanged;

        public SystemActiveLayoutWatcher(ILoggerFactory loggerFactory, ISystemActiveLayoutAdapter adapter, IErrorManager errorManager)
        {
            _logger = loggerFactory.CreateLogger<SystemActiveLayoutWatcher>();
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _cancellationTokenSource = new CancellationTokenSource();

            CheckActiveAppLayout();

            var task = new Task(() => 
            { 
                try
                {
                    WatchActiveAppLayout();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "WatchActiveAppLayout failed");
                }
            }, _cancellationTokenSource.Token);
            task.Start();
        }

        public override void Stop()
        {
            _cancellationTokenSource.Cancel();

            base.Stop();
        }

        public void CheckActiveAppLayout()
        {
            var systemLayout = _adapter.GetCurrentSystemLayoutId();
            if (systemLayout != _currentSystemLayout)
            {
                _currentSystemLayout = systemLayout;

                OnSystemLayoutChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void WatchActiveAppLayout()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    CheckActiveAppLayout();
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        _errorManager.GetFullErrorMessage(ErrorCode.CoreLayoutWatcherFailed)
                    );
                }

                _cancellationTokenSource.Token.WaitHandle.WaitOne(ThreadPollingTimeout);
            }
        }
    }
}
