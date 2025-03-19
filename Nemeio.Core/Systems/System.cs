using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.EventArguments;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Systems.Hid;
using Nemeio.Core.Systems.Sessions;
using Nemeio.Core.Systems.Sleep;
using Nemeio.Core.Systems.Watchers;
using Nemeio.Core.Tools.Stoppable;

namespace Nemeio.Core.Systems
{
    public class System : Stoppable, ISystem
    {
        private const int MaxOsSilentSwitchDelayInMilliSeconds = 1000;
        private const int CheckDelayForOsChange = 50;

        private OsLayoutId _selectedLayout;

        private readonly ILogger _logger;
        private readonly ISystemLayoutLoaderAdapter _loaderAdapter;
        private readonly ISystemActiveLayoutAdapter _activeLayoutAdapter;
        private readonly ISystemLayoutWatcher _systemLayoutWatcher;
        private readonly ISystemActiveLayoutWatcher _activeLayoutWatcher;
        private readonly ISystemLayoutInteractor _systemLayoutInteractor;
        private readonly ISystemForegroundApplicationAdapter _systemForegroundApplicationAdapter;
        private readonly ISystemHidInteractor _hidInteractor;
        private readonly ISystemSessionStateWatcher _sessionStateWatcher;
        private readonly ISleepModeAdapter _sleepModeAdapter;

        private SemaphoreSlim _silentJumpSemaphore = new SemaphoreSlim(1, 1);

        private bool _listenSystemLayoutChange;
        private CancellationTokenSource _waitOsChangeCancellation;

        public event EventHandler OnSelectedLayoutChanged;
        public event EventHandler OnLayoutsChanged;
        public event EventHandler OnForegroundApplicationChanged;
        public event EventHandler OnSessionStateChanged;
        public event EventHandler OnSleepModeChanged;
        public event EventHandler<RemovedByDeletionEventArgs> OnRemovedByHidDeletion;

        public SessionState SessionState { get; private set; } = SessionState.Open;
        public Application ForegroundApplication { get; private set; }
        public OsLayoutId SelectedLayout
        {
            get => _selectedLayout;
            private set
            {
                _selectedLayout = value;

                //  FIXME [KSB] : Modify when manage missing HID layout
                if (value != null)
                {
                    _hidInteractor.SystemLayoutChanged(value);
                }
            }
        }
        public OsLayoutId DefaultLayout => _activeLayoutAdapter.GetDefaultSystemLayoutId();
        public IList<OsLayoutId> Layouts { get; private set; }
        public SleepMode SleepMode { get; private set; } = SleepMode.Awake;

        public System(ILoggerFactory loggerFactory, ISystemActiveLayoutWatcher activeLayoutWatcher, ISystemLayoutLoaderAdapter loaderAdapter, ISystemLayoutInteractor systemLayoutInteractor, ISystemActiveLayoutAdapter activeLayoutAdapter, ISystemLayoutWatcher layoutsWatcher, ISystemHidInteractor hidInteractor, ISystemForegroundApplicationAdapter foregroundAppAdapter, ISystemSessionStateWatcher sessionStateWatcher, ISleepModeAdapter sleepModeAdapter)
        {
            _listenSystemLayoutChange = true;



            _logger = loggerFactory.CreateLogger<System>();
            _loaderAdapter = loaderAdapter ?? throw new ArgumentNullException(nameof(loaderAdapter));
            _systemLayoutInteractor = systemLayoutInteractor ?? throw new ArgumentNullException(nameof(systemLayoutInteractor));
            _activeLayoutAdapter = activeLayoutAdapter ?? throw new ArgumentNullException(nameof(activeLayoutAdapter));

            _activeLayoutWatcher = activeLayoutWatcher ?? throw new ArgumentNullException(nameof(activeLayoutWatcher));
            _activeLayoutWatcher.OnSystemLayoutChanged += ActiveLayoutWatcher_OnSystemLayoutChanged;

            _systemLayoutWatcher = layoutsWatcher ?? throw new ArgumentNullException(nameof(layoutsWatcher));
            _systemLayoutWatcher.LayoutChanged += SystemLayoutWatcher_LayoutChanged;

            _hidInteractor = hidInteractor ?? throw new ArgumentNullException(nameof(hidInteractor));

            _systemForegroundApplicationAdapter = foregroundAppAdapter ?? throw new ArgumentNullException(nameof(foregroundAppAdapter));
            _systemForegroundApplicationAdapter.OnApplicationChanged += SystemForegroundApplicationAdapter_OnApplicationChanged;

            _sessionStateWatcher = sessionStateWatcher ?? throw new ArgumentNullException(nameof(sessionStateWatcher));
            _sessionStateWatcher.StateChanged += SessionStateWatcher_StateChanged;

            _sleepModeAdapter = sleepModeAdapter ?? throw new ArgumentNullException(nameof(sleepModeAdapter));
            _sleepModeAdapter.OnSleepModeChanged += SleepModeAdapter_OnSleepModeChanged;

            LoadSystemLayouts();
            LoadActiveSystemLayout();

            _hidInteractor.SystemLayoutChanged(SelectedLayout);
            _hidInteractor.Run();

            _systemForegroundApplicationAdapter.Start();
        }



        public override void Stop()
        {
            _sessionStateWatcher.StateChanged -= SessionStateWatcher_StateChanged;
            _systemLayoutWatcher.LayoutChanged -= SystemLayoutWatcher_LayoutChanged;
            _activeLayoutWatcher.OnSystemLayoutChanged -= ActiveLayoutWatcher_OnSystemLayoutChanged;
            _sleepModeAdapter.OnSleepModeChanged -= SleepModeAdapter_OnSleepModeChanged;

            _sessionStateWatcher.Dispose();

            _systemForegroundApplicationAdapter.Stop();
            _activeLayoutWatcher.Stop();
            _hidInteractor.Stop();

            base.Stop();
        }

        public async Task EnforceSystemLayoutAsync(OsLayoutId targetOsLayoutId)
        {
            if (targetOsLayoutId != null && SelectedLayout != targetOsLayoutId)
            {
                //  Need to enforce system associated HID layout
                _logger.LogTrace($"need to enforce system associated HID layout <{targetOsLayoutId}>");

                await SilentlyJumpToSystemLayoutIdAsync(targetOsLayoutId);

                SelectedLayout = targetOsLayoutId;
                OnSelectedLayoutChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void PressKeys(IList<string> keys) => _hidInteractor.PostHidStringKeys(keys.ToArray());

        private void LoadSystemLayouts()
        {
            Layouts = _loaderAdapter.Load().ToList();
            OnLayoutsChanged?.Invoke(this, EventArgs.Empty);

            foreach (var layout in Layouts)
            {
                _logger.LogInformation($"* <{layout}> with Name <{layout.Name}> Order <{layout.Order}>");
            }
        }

        private void LoadActiveSystemLayout()
        {
            SelectedLayout = _activeLayoutWatcher.CurrentSystemLayoutId;
            OnSelectedLayoutChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Change System layouts

        private async Task SilentlyJumpToSystemLayoutIdAsync(OsLayoutId systemLayoutId)
        {
            await _silentJumpSemaphore.WaitAsync();

            try
            {
                _listenSystemLayoutChange = false;

                _systemLayoutInteractor.ChangeSelectedLayout(systemLayoutId);

                await WaitLayoutChangeOrTimeoutAsync(systemLayoutId);

                _listenSystemLayoutChange = true;
            }
            finally
            {
                _silentJumpSemaphore.Release();
            }
        }

        private async Task WaitLayoutChangeOrTimeoutAsync(OsLayoutId systemLayoutId)
        {
            _waitOsChangeCancellation = new CancellationTokenSource();

            await Task.WhenAny(
                WaitLayoutChangeAsync(systemLayoutId),
                Task.Delay(MaxOsSilentSwitchDelayInMilliSeconds, _waitOsChangeCancellation.Token)
            );

            _waitOsChangeCancellation.Cancel();
        }

        private async Task WaitLayoutChangeAsync(OsLayoutId systemLayoutId)
        {
            while (!_waitOsChangeCancellation.IsCancellationRequested)
            {
                if (SelectedLayout.ToString() == systemLayoutId.ToString())
                {
                    _waitOsChangeCancellation.Cancel();
                    _logger.LogInformation($"Silent jump to OS layout <{SelectedLayout}, {SelectedLayout.Name}> now complete");

                    return;
                }
                else
                {
                    await Task.Delay(CheckDelayForOsChange);
                }
            }

            _logger.LogInformation($"Silent jump to OS layout <{systemLayoutId}, {systemLayoutId.Name}> TIMEDOUT");
        }

        #endregion

        #region Events 

        private void SystemLayoutWatcher_LayoutChanged(object sender, EventArgs e) => LoadSystemLayouts();

        private void ActiveLayoutWatcher_OnSystemLayoutChanged(object sender, EventArgs e)
        {
            if (_listenSystemLayoutChange)
            {
                LoadActiveSystemLayout();
            }
        }

        private void SystemForegroundApplicationAdapter_OnApplicationChanged(object sender, ApplicationChangedEventArgs e)
        {
            _logger.LogInformation($"New application on focus <{e.Application.Name}>");

            ForegroundApplication = e.Application;
            OnForegroundApplicationChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SessionStateWatcher_StateChanged(object sender, SessionStateChangedEventArgs e)
        {
            _logger.LogInformation($"Session state changed <{e.State}>");

            SessionState = e.State;
            OnSessionStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SleepModeAdapter_OnSleepModeChanged(object sender, SleepModeChangedEventArgs e)
        {
            _logger.LogInformation($"Sleep mode changed <{e.Mode}>");

            SleepMode = e.Mode;
            OnSleepModeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyCustomRemovedByHid(List<ILayout> hidDeleted, List<ILayout> customLayoutsToDisable)
        {
            OnRemovedByHidDeletion?.Invoke(this, new RemovedByDeletionEventArgs(hidDeleted.Select(x => x.Title).ToArray<string>(), customLayoutsToDisable.Select(x => x.Title).ToArray<string>()));
        }

        public void CheckForegroundApplication()
        {
            _systemForegroundApplicationAdapter.Stop();
            _systemForegroundApplicationAdapter.Start();
        }

        #endregion
    }
}
