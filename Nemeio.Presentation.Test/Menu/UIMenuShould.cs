using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Applications;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.Icon;
using Nemeio.Core.Managers;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Batteries;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Settings;
using Nemeio.Core.Systems;
using Nemeio.Presentation.Menu;
using Nemeio.Presentation.Menu.Administrator;
using Nemeio.Presentation.Menu.Synchronization;
using Nemeio.Presentation.Menu.Tools;
using Nemeio.Presentation.Modals;
using Nemeio.Presentation.PackageUpdater.UI;
using NUnit.Framework;

namespace Nemeio.Presentation.Test.Menu
{
    public class UIMenuShould
    {
        private ISystem _system;
        private IApplicationMenu _applicationMenu;
        private IDialogService _dialogService;
        private ILanguageManager _languageManager;
        private IPackageUpdaterMessageFactory _messageFactory;
        private IModalWindowFactory _modalWindowFactory;
        private IScreenResolutionAdapter _screenResolutionAdapter;
        private IAdministratorModalStrategyFactory _administratorModalStrategyFactory;
        private ISettingsHolder _settingsHolder;
        private IApplicationSettingsProvider _applicationSettingsProvider;
        private IApplicationManifest _applicationManifest;

        private ObservableValue<Core.Keyboard.Nemeios.INemeio> _keyboard;
        private ObservableValue<BatteryInformation> _batteryInformation;
        private ObservableValue<ApplicationIconType> _icon;
        private ObservableValue<ILayout> _selectedLayout;
        private ObservableValue<IList<ILayout>> _layouts;
        private ObservableValue<bool> _applicationAugmentedHidEnable;
        private ObservableValue<bool> _syncing;
        private ObservableValue<SynchronizationProgress> _syncProgress;
        private ObservableValue<PackageUpdateState> _updateKind;
        private ObservableValue<Version> _applicationVersion;
        private ObservableValue<Core.Systems.Applications.Application> _foregroundApplication;

        private IModalWindow _updateModal;
        private IModalWindow _keyboardInitErrorModal;
        private IModalWindow _administratorModal;

        private UIMenu _uiMenu;

        [SetUp]
        public void SetUp()
        {
            var loggerFactory = new LoggerFactory();

            _system = Mock.Of<ISystem>();
            _applicationMenu = Mock.Of<IApplicationMenu>();
            _dialogService = Mock.Of<IDialogService>();

            _languageManager = Mock.Of<ILanguageManager>();
            Mock.Get(_languageManager)
                .Setup(x => x.GetLocalizedValue(Core.DataModels.Locale.StringId.CommonVersion))
                .Returns("Version");

            _messageFactory = Mock.Of<IPackageUpdaterMessageFactory>();
            _modalWindowFactory = Mock.Of<IModalWindowFactory>();
            _screenResolutionAdapter = Mock.Of<IScreenResolutionAdapter>();
            _administratorModalStrategyFactory = Mock.Of<IAdministratorModalStrategyFactory>();
            _settingsHolder = Mock.Of<ISettingsHolder>();
            _applicationSettingsProvider = Mock.Of<IApplicationSettingsProvider>();
            _applicationManifest = Mock.Of<IApplicationManifest>();

            _keyboard = new ObservableValue<Core.Keyboard.Nemeios.INemeio>();
            _batteryInformation = new ObservableValue<BatteryInformation>();
            _icon = new ObservableValue<ApplicationIconType>();
            _selectedLayout = new ObservableValue<ILayout>();
            _layouts = new ObservableValue<IList<ILayout>>();
            _applicationAugmentedHidEnable = new ObservableValue<bool>();
            _syncing = new ObservableValue<bool>();
            _syncProgress = new ObservableValue<SynchronizationProgress>();
            _updateKind = new ObservableValue<PackageUpdateState>();
            _applicationVersion = new ObservableValue<System.Version>();
            _foregroundApplication = new ObservableValue<Core.Systems.Applications.Application>();

            Mock.Get(_applicationMenu)
                .Setup(x => x.Keyboard)
                .Returns(_keyboard);
            Mock.Get(_applicationMenu)
                .Setup(x => x.BatteryInformations)
                .Returns(_batteryInformation);
            Mock.Get(_applicationMenu)
                .Setup(x => x.Icon)
                .Returns(_icon);
            Mock.Get(_applicationMenu)
                .Setup(x => x.SelectedLayout)
                .Returns(_selectedLayout);
            Mock.Get(_applicationMenu)
                .Setup(x => x.Layouts)
                .Returns(_layouts);
            Mock.Get(_applicationMenu)
                .Setup(x => x.ApplicationAugmentedHidEnable)
                .Returns(_applicationAugmentedHidEnable);
            Mock.Get(_applicationMenu)
                .Setup(x => x.Syncing)
                .Returns(_syncing);
            Mock.Get(_applicationMenu)
                .Setup(x => x.SyncProgression)
                .Returns(_syncProgress);
            Mock.Get(_applicationMenu)
                .Setup(x => x.UpdateKind)
                .Returns(_updateKind);
            Mock.Get(_applicationMenu)
                .Setup(x => x.ApplicationVersion)
                .Returns(_applicationVersion);
            Mock.Get(_applicationMenu)
                .Setup(x => x.ForegroundApplication)
                .Returns(_foregroundApplication);

            _updateModal = Mock.Of<IModalWindow>();
            _keyboardInitErrorModal = Mock.Of<IModalWindow>();

            Mock.Get(_modalWindowFactory)
                .Setup(x => x.CreateUpdateModal())
                .Returns(_updateModal);

            Mock.Get(_modalWindowFactory)
                .Setup(x => x.CreateKeyboardInitErrorModal())
                .Returns(_keyboardInitErrorModal);

            Mock.Get(_modalWindowFactory)
                .Setup(x => x.CreateAdministratorModal(It.IsAny<Core.Systems.Applications.Application>()))
                .Returns(_administratorModal);

            _uiMenu = new UIMenu(loggerFactory, _system, _applicationSettingsProvider, _applicationMenu, _dialogService, _languageManager, _messageFactory, _modalWindowFactory, _screenResolutionAdapter, _administratorModalStrategyFactory, _settingsHolder, _applicationManifest);
        }

        [Test]
        public void UIMenu_DisplayUpdateModalIfNeeded_WhenUpdateKind_IsError_DoNothing()
        {
            var displayCalled = false;

            Mock.Get(_updateModal)
                .Setup(x => x.Display())
                .Callback(() => displayCalled = true);

            _updateKind.Value = PackageUpdateState.UpdateFailed;

            _uiMenu.DisplayUpdateModalIfNeeded();

            displayCalled.Should().BeTrue();
        }

        [Test]
        public void UIMenu_DisplayUpdateModalIfNeeded_WhenUpdateKind_IsNoUpdate_DoNothing()
        {
            var displayCalled = false;

            Mock.Get(_updateModal)
                .Setup(x => x.Display())
                .Callback(() => displayCalled = true);

            _updateKind.Value = PackageUpdateState.Idle;

            _uiMenu.DisplayUpdateModalIfNeeded();

            displayCalled.Should().BeFalse();
        }

        [TestCase(PackageUpdateState.Downloading)]
        [TestCase(PackageUpdateState.DownloadPending)]
        [TestCase(PackageUpdateState.UpdatePending)]
        [TestCase(PackageUpdateState.UpdateSucceed)]
        public void UIMenu_DisplayUpdateModalIfNeeded_WhenUpdateKind_CanShowModal_DisplayModal(PackageUpdateState kind)
        {
            var displayCalled = false;

            Mock.Get(_updateModal)
                .Setup(x => x.Display())
                .Callback(() => displayCalled = true);

            _updateKind.Value = kind;

            _uiMenu.DisplayUpdateModalIfNeeded();

            displayCalled.Should().BeTrue();
        }

        [Test]
        public void UIMenu_DisplayKeyboardInitErrorModal_WhenKeyboardInitFailed_Ok()
        {
            var displayCalled = false;

            Mock.Get(_keyboardInitErrorModal)
                .Setup(x => x.Display())
                .Callback(() => displayCalled = true);

            Mock.Get(_applicationMenu)
                .Raise(x => x.OnKeyboardInitFailed += null, EventArgs.Empty);

            displayCalled.Should().BeTrue();
        }
    }
}
