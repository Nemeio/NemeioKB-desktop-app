using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MvvmCross.Platform;
using Nemeio.Core;
using Nemeio.Core.Applications;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.EventArguments;
using Nemeio.Core.Managers;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Settings;
using Nemeio.Core.Systems;
using Nemeio.Presentation.Menu.Administrator;
using Nemeio.Presentation.Menu.Icon;
using Nemeio.Presentation.Menu.Synchronization;
using Nemeio.Presentation.Menu.Tools;
using Nemeio.Presentation.Modals;
using Nemeio.Presentation.PackageUpdater.UI;
using Newtonsoft.Json;

namespace Nemeio.Presentation.Menu
{
    public enum MenuState
    {
        Open,
        Close
    }

    public class UIMenu : IUIMenu, IApplicationMenuUIDelegate
    {
        private readonly ILogger _logger;
        private readonly ISystem _system;
        private readonly IApplicationMenu _applicationMenu;
        private readonly IDialogService _dialogService;
        private readonly ILanguageManager _languageManager;
        private readonly IPackageUpdaterMessageFactory _messageFactory;
        private readonly IModalWindowFactory _modalWindowFactory;
        private readonly IScreenResolutionAdapter _screenResolutionAdapter;
        private readonly ISettingsHolder _settings;
        private readonly IApplicationSettingsProvider _applicationSettingsProvider;
        private readonly IApplicationManifest _applicationManifest;
        private readonly ApplicationIconBuilder _applicationIconBuilder;

        private IModalWindow _updateModal;
        private IModalWindow _quitModal;
        private IModalWindow _languageSelectionModal;
        private IModalWindow _configuratorModal;
        private IModalWindow _factoryResetModal;
        private IModalWindow _keyboardInitErrorModal;
        private IModalWindow _removedByHidModal;

        private readonly IAdministratorModalStrategy _administratorModalStrategy;

        #region Sections

        public ObservableValue<ApplicationIcon> Icon { get; private set; } = new ObservableValue<ApplicationIcon>();
        public ObservableValue<MenuState> State { get; private set; } = new ObservableValue<MenuState>();
        public ObservableValue<Menu> Menu { get; private set; } = new ObservableValue<Menu>();

        #endregion

        public UIMenu(ILoggerFactory loggerFactory, ISystem system, IApplicationSettingsProvider applicationSettingsProvider, IApplicationMenu applicationMenu, IDialogService dialogService, ILanguageManager languageManager, IPackageUpdaterMessageFactory messageFactory, IModalWindowFactory modalWindowFactory, IScreenResolutionAdapter screenResolutionAdapter, IAdministratorModalStrategyFactory administratorModalStrategyFactory, ISettingsHolder settings, IApplicationManifest applicationManifest)
        {
            _logger = loggerFactory.CreateLogger<UIMenu>();
            _system = system ?? throw new ArgumentNullException(nameof(system));

            _system.OnSessionStateChanged += System_OnSessionStateChanged;
            _system.OnRemovedByHidDeletion += _system_OnRemovedByHidDeletion;
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _languageManager.LanguageChanged += LanguageManager_LanguageChanged;
            _languageManager.LanguageDueCare += LanguageManager_LanguageDueCare;

            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _modalWindowFactory = modalWindowFactory ?? throw new ArgumentNullException(nameof(modalWindowFactory));

            _applicationIconBuilder = new ApplicationIconBuilder(_languageManager, _messageFactory);

            _updateModal = _modalWindowFactory.CreateUpdateModal();
            _quitModal = _modalWindowFactory.CreateQuitModal();
            _languageSelectionModal = _modalWindowFactory.CreateLanguageSelectionModal();
            _configuratorModal = _modalWindowFactory.CreateConfiguratorModal();
            _factoryResetModal = _modalWindowFactory.CreateFactoryResetModal();
            _keyboardInitErrorModal = _modalWindowFactory.CreateKeyboardInitErrorModal();
            _removedByHidModal = _modalWindowFactory.CreateRemovedByHidModal();

            _administratorModalStrategy = administratorModalStrategyFactory.Create();

            _screenResolutionAdapter = screenResolutionAdapter ?? throw new ArgumentNullException(nameof(screenResolutionAdapter));
            _screenResolutionAdapter.OnScreenResolutionChanged += ScreenResolutionAdapter_OnScreenResolutionChanged;

            _applicationMenu = applicationMenu ?? throw new ArgumentNullException(nameof(applicationMenu));
            _applicationMenu.UIDelegate = this;
            _applicationMenu.Layouts.OnValueChanged += Layouts_OnValueChanged;
            _applicationMenu.SelectedLayout.OnValueChanged += SelectedLayout_OnValueChanged;
            _applicationMenu.Syncing.OnValueChanged += Syncing_OnValueChanged;
            _applicationMenu.SyncProgression.OnValueChanged += SyncProgression_OnValueChanged;
            _applicationMenu.ApplicationAugmentedHidEnable.OnValueChanged += ApplicationAugmentedHidEnable_OnValueChanged;
            _applicationMenu.BatteryInformations.OnValueChanged += BatteryInformations_OnValueChanged;
            _applicationMenu.Icon.OnValueChanged += Icon_OnValueChanged;
            _applicationMenu.Keyboard.OnValueChanged += Keyboard_OnValueChanged;
            _applicationMenu.ApplicationVersion.OnValueChanged += ApplicationVersion_OnValueChanged;
            _applicationMenu.UpdateKind.OnValueChanged += UpdateKind_OnValueChanged;
            _applicationMenu.OnFactoryResetStarted += ApplicationMenu_OnFactoryResetStarted;
            _applicationMenu.OnKeyboardInitFailed += ApplicationMenu_OnKeyboardInitFailed;
            _applicationMenu.ForegroundApplication.OnValueChanged += ForegroundApplication_OnValueChanged;

            _applicationSettingsProvider = applicationSettingsProvider ?? throw new ArgumentNullException(nameof(applicationSettingsProvider));
            _applicationManifest = applicationManifest;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _settings.OnSettingsUpdated += Settings_OnSettingsUpdated;

            Menu.Value = new Menu(_languageManager, _messageFactory);

            //  Set default value
            State.Value = MenuState.Close;
        }
        private void _system_OnRemovedByHidDeletion(object sender, RemovedByDeletionEventArgs e)
        {
            string[] args = new string[3];
            args[0] = Environment.NewLine;
            args[1] = String.Join(Environment.NewLine, e.DeletedHidName);
            args[2] = String.Join(Environment.NewLine, e.RemovedCustomNames);
            _dialogService.ShowMessageWithArgs(StringId.CommonCustom, StringId.CustomLayoutRemovedByHidDeletion, args);
        }
        private void Settings_OnSettingsUpdated(object sender, EventArgs e)
        {
            _dialogService.ShowNotification(StringId.ApplicationTitleName, StringId.DevSettingsChangedMessage);
        }

        private void System_OnSessionStateChanged(object sender, EventArgs e)
        {
            DisplayUpdateModalIfNeeded();
        }

        private void RefreshMenu()
        {
            Menu.Value = Menu?.Value?.Build(_applicationMenu);
            RefreshIcon();
        }

        #region Events

        private void ApplicationMenu_OnFactoryResetStarted(object sender, EventArgs e)
        {
            DisplayModalOrFocus(_factoryResetModal);
        }

        private void UpdateKind_OnValueChanged(object sender, ObservableValueChangedEventArgs<PackageUpdateState> e)
        {
            RefreshMenu();

            if (e.Value == PackageUpdateState.UpdateFailed)
            {
                _applicationSettingsProvider.LastRollbackManifestString = JsonConvert.SerializeObject(_applicationManifest.FirmwareManifest);
            }
            if (e.Value == PackageUpdateState.UpdateSucceed)
            {
                _applicationSettingsProvider.LastRollbackManifestString = String.Empty;
            }



            if ((e.Value == PackageUpdateState.UpdatePending && !HasBeenRolledBack()) ||
                e.Value == PackageUpdateState.DownloadPending ||
                e.Value == PackageUpdateState.UpdateFailed ||
                e.Value == PackageUpdateState.UpdateSucceed)
            {
                DisplayModalOrFocus(_updateModal);
            }
        }

        private bool HasBeenRolledBack()
        {
            try
            {
                var targetManifestString = JsonConvert.SerializeObject(_applicationManifest.FirmwareManifest);
                var lastRollback = _applicationSettingsProvider.ApplicationSettings.LastRollbackManifestString;

                return targetManifestString == lastRollback;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Can't check if Firmware has already been rollback, popup window will be shown");
                return false;
            }


        }

        private void Keyboard_OnValueChanged(object sender, ObservableValueChangedEventArgs<Core.Keyboard.Nemeios.INemeio> e)
        {
            RefreshMenu();
        }

        private void BatteryInformations_OnValueChanged(object sender, ObservableValueChangedEventArgs<Core.Services.Batteries.BatteryInformation> e)
        {
            //  Battery have changed
            //  We rebuild battery section

            RefreshMenu();
        }

        private void ApplicationVersion_OnValueChanged(object sender, ObservableValueChangedEventArgs<System.Version> e)
        {
            RefreshMenu();
        }

        #endregion

        public void Run()
        {
            _applicationMenu.Run();

            RefreshMenu();
        }

        #region Application 

        private void ForegroundApplication_OnValueChanged(object sender, ObservableValueChangedEventArgs<Core.Systems.Applications.Application> e)
        {
            _administratorModalStrategy.OnForegroundApplicationChanged(e.Value);
        }

        #endregion

        #region Keyboard

        private void ApplicationMenu_OnKeyboardInitFailed(object sender, EventArgs e) => DisplayModalOrFocus(_keyboardInitErrorModal);

        #endregion

        #region Layouts

        public Task SelectLayoutAsync(ILayout layout) => _applicationMenu.SelectLayoutAsync(layout);

        private void SelectedLayout_OnValueChanged(object sender, ObservableValueChangedEventArgs<Core.Services.Layouts.ILayout> e)
        {
            RefreshMenu();
        }

        private void Layouts_OnValueChanged(object sender, ObservableValueChangedEventArgs<System.Collections.Generic.IList<Core.Services.Layouts.ILayout>> e)
        {
            RefreshMenu();
        }

        private void Syncing_OnValueChanged(object sender, ObservableValueChangedEventArgs<bool> e)
        {
            RefreshMenu();
        }

        private void SyncProgression_OnValueChanged(object sender, ObservableValueChangedEventArgs<SynchronizationProgress> e)
        {
            RefreshMenu();
        }

        private void ApplicationAugmentedHidEnable_OnValueChanged(object sender, ObservableValueChangedEventArgs<bool> e)
        {
            RefreshMenu();
        }

        #endregion

        #region Icon

        private void RefreshIcon()
        {
            Icon.Value = _applicationIconBuilder.Build(_applicationMenu.Icon.Value);
        }

        private void Icon_OnValueChanged(object sender, ObservableValueChangedEventArgs<Core.Icon.ApplicationIconType> e)
        {
            //  Icon must change
            //  We rebuild it

            RefreshMenu();
        }

        #endregion

        #region Language

        private void LanguageManager_LanguageChanged(object sender, EventArgs e)
        {
            //  Language changed
            //  We must refresh every section

            RefreshMenu();
        }

        private void LanguageManager_LanguageDueCare(object sender, EventArgs e)
        {
            //  Language must be selected by user
            //  Prompt modal

            DisplayModalOrFocus(_languageSelectionModal);
        }

        #endregion

        #region Update

        public void DisplayUpdateModalIfNeeded()
        {
            if (_applicationMenu != null && _applicationMenu.UpdateKind != null)
            {
                var updateKind = _applicationMenu.UpdateKind.Value;
                if (updateKind != PackageUpdateState.UpdateFailed &&
                    updateKind != PackageUpdateState.Idle &&
                    updateKind != PackageUpdateState.UpdateChecking &&
                    updateKind != PackageUpdateState.CheckInternetConnection &&
                    updateKind != PackageUpdateState.CheckApplicationUpdate &&
                    updateKind != PackageUpdateState.CheckFirmwareUpdate &&
                    updateKind != PackageUpdateState.UpdateSucceed)
                {
                    DisplayModalOrFocus(_updateModal);
                }
            }
        }

        #endregion

        #region Factory Reset

        //public abstract void DisplayFactoryResetModal();

        #endregion

        #region Quit

        public void DisplayQuitModal() => DisplayModalOrFocus(_quitModal);

        #endregion

        #region Configurator

        public void DisplayConfiguratorModal() => DisplayModalOrFocus(_configuratorModal);

        #endregion

        #region Tools

        private void DisplayModalOrFocus(IModalWindow modalWindow)
        {
            _logger.LogInformation($"Display modal <{modalWindow}>");

            if (!modalWindow.IsOpen)
            {
                modalWindow.OnClosing += Modal_OnClosing;
                modalWindow.Display();
            }
            else
            {
                modalWindow.Focus();
            }
        }

        private void Modal_OnClosing(object sender, OnClosingModalEventArgs e)
        {
            e.ModalWindow.OnClosing -= Modal_OnClosing;
        }

        #endregion

        #region UI Delegate

        public async Task DisplayDialogAsync(StringId title, StringId message)
        {
            await Task.Yield();

            _dialogService.ShowMessage(title, message);
        }

        public async Task DisplayNotificationAsync(StringId title, StringId message)
        {
            await Task.Yield();

            _dialogService.ShowNotification(title, message);
        }

        #endregion

        #region Resolution

        private void ScreenResolutionAdapter_OnScreenResolutionChanged(object sender, EventArgs e)
        {
            CloseMenu();
        }

        #endregion

        #region Menu State

        public void OpenMenu() => State.Value = MenuState.Open;

        public void CloseMenu() => State.Value = MenuState.Close;

        #endregion
    }
}
