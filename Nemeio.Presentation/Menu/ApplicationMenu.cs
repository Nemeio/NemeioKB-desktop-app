using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Applications;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.EventArguments;
using Nemeio.Core.Icon;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Battery;
using Nemeio.Core.Keyboard.Configurations;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keyboard.State;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.Layouts.Synchronization.Contexts.State;
using Nemeio.Core.Managers;
using Nemeio.Core.Notifier;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Batteries;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;
using Nemeio.Core.Tools.StateMachine;
using Nemeio.Presentation.Menu.Synchronization;
using Nemeio.Presentation.Menu.Tools;

namespace Nemeio.Presentation.Menu
{
    public sealed class ApplicationMenu : IApplicationMenu
    {
        private sealed class ConfigurableNemeioProxy : KeyboardProxy, IConfigurationHolder
        {
            private readonly IConfigurationHolder _configurationHolder;

            public IList<LayoutIdWithHash> LayoutIdWithHashs => _configurationHolder.LayoutIdWithHashs;
            public LayoutId SelectedLayoutId => _configurationHolder.SelectedLayoutId;
            public IScreen Screen => _configurationHolder.Screen;

            public event EventHandler OnSelectedLayoutChanged;

            public ConfigurableNemeioProxy(Core.Keyboard.Nemeios.INemeio nemeio)
                : base(nemeio)
            {
                _configurationHolder = nemeio as IConfigurationHolder;
                _configurationHolder.OnSelectedLayoutChanged += ConfigurationHolder_OnSelectedLayoutChanged;
            }

            private void ConfigurationHolder_OnSelectedLayoutChanged(object sender, EventArgs e) => OnSelectedLayoutChanged?.Invoke(sender, e);

            public Task StartSynchronizationAsync() => _configurationHolder.StartSynchronizationAsync();
            public Task EndSynchronizationAsync() => _configurationHolder.EndSynchronizationAsync();
        }

        private sealed class BatteryHolderNemeioProxy : KeyboardProxy, IBatteryHolder
        {
            private readonly IBatteryHolder _batteryHolder;

            public BatteryInformation Battery => _batteryHolder.Battery;

            public event EventHandler OnBatteryLevelChanged;

            public BatteryHolderNemeioProxy(Core.Keyboard.Nemeio nemeio)
                : base(nemeio)
            {
                _batteryHolder = nemeio as IBatteryHolder;
                _batteryHolder.OnBatteryLevelChanged += BatteryHolder_OnBatteryLevelChanged;
            }

            private void BatteryHolder_OnBatteryLevelChanged(object sender, EventArgs e) => OnBatteryLevelChanged?.Invoke(sender, e);
        }

        private readonly ILogger _logger;
        private readonly ISystem _system;
        private readonly IInformationService _informationService;
        private readonly ILanguageManager _languageManager;
        private readonly IKeyboardController _keyboardController;
        private readonly IApplicationSettingsProvider _applicationSettingsManager;
        private readonly IPackageUpdater _packageUpdater;
        private readonly IApplicationIconProvider _iconProvider;
        private readonly ILayoutLibrary _library;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;
        private readonly ISynchronizer _synchronizer;

        private ISynchronizationContextState _syncContextState;
        private BatteryNotifier _batteryNotifier;

        private BatteryHolderNemeioProxy _batteryProxy;

        public ObservableValue<ApplicationIconType> Icon { get; private set; } = new ObservableValue<ApplicationIconType>();
        public ObservableValue<BatteryInformation> BatteryInformations { get; private set; } = new ObservableValue<BatteryInformation>();
        public ObservableValue<Nemeio.Core.Keyboard.Nemeios.INemeio> Keyboard { get; private set; } = new ObservableValue<Core.Keyboard.Nemeios.INemeio>();
        public ObservableValue<bool> Syncing { get; private set; } = new ObservableValue<bool>();
        public ObservableValue<SynchronizationProgress> SyncProgression { get; private set; } = new ObservableValue<SynchronizationProgress>();
        public ObservableValue<ILayout> SelectedLayout { get; private set; } = new ObservableValue<ILayout>();
        public ObservableValue<IList<ILayout>> Layouts { get; private set; } = new ObservableValue<IList<ILayout>>();
        public ObservableValue<bool> ApplicationAugmentedHidEnable { get; private set; } = new ObservableValue<bool>();
        public ObservableValue<PackageUpdateState> UpdateKind { get; private set; } = new ObservableValue<PackageUpdateState>();
        public ObservableValue<System.Version> ApplicationVersion { get; private set; } = new ObservableValue<System.Version>();
        public ObservableValue<Core.Systems.Applications.Application> ForegroundApplication { get; private set; } = new ObservableValue<Core.Systems.Applications.Application>();

        public IApplicationMenuUIDelegate UIDelegate { get; set; }

        public event EventHandler OnFactoryResetStarted;
        public event EventHandler OnKeyboardInitFailed;

        public ApplicationMenu(ILoggerFactory loggerFactory, ISystem system, ILanguageManager languageManager, IKeyboardController keyboardController, IApplicationSettingsProvider applicationSettingsManager, IPackageUpdater packageUpdater, IApplicationIconProvider iconProvider, ILayoutLibrary library, IActiveLayoutChangeHandler activeLayoutChangeHandler, ISynchronizer synchronizer, IInformationService informationService)
        {
            _logger = loggerFactory.CreateLogger<ApplicationMenu>();

            _system = system ?? throw new ArgumentNullException(nameof(system));
            _system.OnForegroundApplicationChanged += System_OnForegroundApplicationChanged;

            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));

            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _keyboardController.OnKeyboardConnected += KeyboardController_OnKeyboardConnected;
            _keyboardController.OnKeyboardDisconnecting += KeyboardController_OnKeyboardDisconnecting;
            _keyboardController.OnKeyboardDisconnected += KeyboardController_OnKeyboardDisconnected;
            _keyboardController.OnKeyboardInitializationFailed += KeyboardController_OnKeyboardInitializationFailed;

            _applicationSettingsManager = applicationSettingsManager ?? throw new ArgumentNullException(nameof(applicationSettingsManager));
            _applicationSettingsManager.AugmentedImageEnableChanged += ApplicationSettingsManager_AugmentedImageEnableChanged;

            _packageUpdater = packageUpdater ?? throw new ArgumentNullException(nameof(packageUpdater));
            _packageUpdater.OnStateChanged += PackageUpdater_OnStateChanged;

            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
            _activeLayoutChangeHandler.Synchronizer.ActiveLayoutChanged += ActiveLayoutSynchronizer_ActiveLayoutChanged;

            _synchronizer = synchronizer ?? throw new ArgumentNullException(nameof(synchronizer));
            _synchronizer.OnStateChanged += Synchronizer_OnStateChanged;

            _iconProvider = iconProvider ?? throw new ArgumentNullException(nameof(iconProvider));
            _library = library ?? throw new ArgumentNullException(nameof(library));
            _informationService = informationService ?? throw new ArgumentNullException(nameof(informationService));
        }

        public void Run()
        {
            Icon.Value = _iconProvider.GetIconFromCurrentState();
            BatteryInformations.Value = null;
            RefreshLayouts();
            SelectedLayout.Value = _activeLayoutChangeHandler.Synchronizer.LastSynchronizedLayout;
            Keyboard.Value = null;
            ApplicationVersion.Value = new System.Version(_informationService.GetApplicationVersion().ToString());

            CheckKeyboardConnect();
        }

        #region System

        private void System_OnForegroundApplicationChanged(object sender, EventArgs e) => ForegroundApplication.Value = _system.ForegroundApplication;

        #endregion

        #region Keyboard

        private void Runner_OnStateChanged(object sender, StateChangedEventArgs e)
        {
            Keyboard.Value = e.State == NemeioState.Disconnecting
                ? null
                : e.Nemeio;

            if (e.State == NemeioState.FactoryReset)
            {
                OnFactoryResetStarted?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CheckKeyboardConnect()
        {
            if (_keyboardController.Connected)
            {
                var proxy = KeyboardProxy.CastTo<BatteryHolderNemeioProxy>(_keyboardController.Nemeio);
                if (proxy != null)
                {
                    _batteryProxy = proxy;

                    _batteryProxy.OnBatteryLevelChanged += Runner_OnBatteryLevelChanged;
                    _batteryProxy.OnStateChanged += Runner_OnStateChanged;

                    _batteryNotifier = new BatteryNotifier(_batteryProxy);
                    _batteryNotifier.BatteryInformationChanged += BatteryNotifier_BatteryInformationsChanged;
                    _batteryNotifier.NotificationReleased += BatteryNotifier_NotificationReleased;
                    _batteryNotifier.Start();

                    Keyboard.Value = _keyboardController.Nemeio;
                }
            }
        }

        private void KeyboardController_OnKeyboardConnected(object sender, EventArgs e)
        {
            CheckKeyboardConnect();
        }

        private void KeyboardController_OnKeyboardDisconnecting(object sender, EventArgs e)
        {
            if (_batteryProxy != null)
            {
                if (_syncContextState != null)
                {
                    _syncContextState.StateChanged -= SyncState_StateChanged;
                    _syncContextState.ProgressionChanged -= ApplicationMenu_ProgressionChanged;
                }

                _batteryProxy.OnBatteryLevelChanged -= Runner_OnBatteryLevelChanged;
            }

            if (_batteryNotifier != null)
            {
                _batteryNotifier.Stop();
                _batteryNotifier.BatteryInformationChanged -= BatteryNotifier_BatteryInformationsChanged;
                _batteryNotifier.NotificationReleased -= BatteryNotifier_NotificationReleased;
            }
        }

        private void KeyboardController_OnKeyboardDisconnected(object sender, EventArgs e)
        {
            Keyboard.Value = null;
            SelectedLayout.Value = null;
            Icon.Value = _iconProvider.GetIconFromCurrentState();
            RefreshBattery();
        }

        private void KeyboardController_OnKeyboardInitializationFailed(object sender, Core.Keyboard.Connection.KeyboardInitializationFailedEventArgs e) => OnKeyboardInitFailed?.Invoke(this, EventArgs.Empty);

        private void SyncState_StateChanged(object sender, EventArgs e)
        {
            Syncing.Value = _syncContextState.State == SynchronizationState.Start || _syncContextState.State == SynchronizationState.InProgress;
        }

        private void ApplicationMenu_ProgressionChanged(object sender, SynchronizationProgessChanged e)
        {
            SyncProgression.Value = new SynchronizationProgress(e.Progress, e.Size);
        }

        #endregion

        #region Synchronization

        private void Synchronizer_OnStateChanged(object sender, EventArgs e)
        {
            Icon.Value = _iconProvider.GetIconFromCurrentState();

            RefreshLayouts();

            if (_syncContextState == null && _keyboardController.Connected)
            {
                _syncContextState = _synchronizer.GetSynchronizationStateFor(_keyboardController.Nemeio);
                if (_syncContextState != null)
                {
                    _syncContextState.StateChanged += SyncState_StateChanged;
                    _syncContextState.ProgressionChanged += ApplicationMenu_ProgressionChanged;
                }
            }

            if (_synchronizer.State == Synchronizer.SynchornizerState.Waiting)
            {
                SelectedLayout.Value = _activeLayoutChangeHandler.Synchronizer.LastSynchronizedLayout;
            }
        }

        private void RefreshLayouts()
        {
            var layouts = Enumerable.Empty<ILayout>().ToList();

            if (Keyboard.Value != null)
            {
                var proxy = KeyboardProxy.CastTo<ConfigurableNemeioProxy>(Keyboard.Value);
                if (proxy != null)
                {
                    layouts = _library
                        .Layouts
                        .Where(layout => layout.Enable)
                        .Where(layout => proxy.LayoutIdWithHashs?.Any(x => x.Id == layout.LayoutId) ?? false)
                        .ToList();

                }
                foreach (var l in layouts)
                {
                    var sysLayout = _system.Layouts.FirstOrDefault(x => x.Id == l.LayoutInfo.OsLayoutId);
                    if (sysLayout != null)
                    {
                        l.Order = sysLayout.Order;
                    }
                }

                var r = _system.Layouts;
            }

            Layouts.Value = layouts.OrderBy(x => x.Order).ToList();
        }

        #endregion

        #region Active layout

        public async Task SelectLayoutAsync(ILayout layout)
        {
            if (layout != null)
            {
                await _activeLayoutChangeHandler.RequestMenuChangeAsync(_keyboardController.Nemeio, layout.LayoutId);
            }
        }

        private void ActiveLayoutSynchronizer_ActiveLayoutChanged(object sender, EventArgs e)
        {
            var layoutSync = _activeLayoutChangeHandler.Synchronizer.LastSynchronizedLayout;

            SelectedLayout.Value = layoutSync;
        }

        #endregion

        #region Augmented Image

        private void ApplicationSettingsManager_AugmentedImageEnableChanged(object sender, EventArgs e)
        {
            ApplicationAugmentedHidEnable.Value = _applicationSettingsManager.AugmentedImageEnable;
        }

        #endregion

        #region Update

        private void PackageUpdater_OnStateChanged(object sender, OnStateChangedEventArgs<PackageUpdateState> e)
        {
            UpdateKind.Value = _packageUpdater.State;
            Icon.Value = _iconProvider.GetIconFromCurrentState();
        }

        #endregion

        #region Battery

        private void RefreshBattery()
        {
            if (_keyboardController.Nemeio != null)
            {
                BatteryInformations.Value = _keyboardController.Nemeio.Battery;
            }
            else
            {
                BatteryInformations.Value = null;
            }
        }

        private void Runner_OnBatteryLevelChanged(object sender, EventArgs e) => RefreshBattery();

        private async void BatteryNotifier_NotificationReleased(object sender, NotificationReleaseEventArgs e)
        {
            if (e != null)
            {
                await UIDelegate?.DisplayNotificationAsync(StringId.BatteryTitleNotification, e.Message);
            }
        }

        private void BatteryNotifier_BatteryInformationsChanged(object sender, BatteryInformationsChangedEventArgs e)
        {
            if (e != null)
            {
                BatteryInformations.Value = e.Informations;
            }
        }

        #endregion
    }
}
