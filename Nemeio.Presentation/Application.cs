using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.IoC;
using Nemeio.Acl.HttpComm;
using Nemeio.Acl.HttpComm.Connectivity;
using Nemeio.Acl.HttpComm.PackageUpdater;
using Nemeio.Core;
using Nemeio.Core.Applications;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.Connectivity;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Errors;
using Nemeio.Core.FactoryReset;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Images.Jpeg.Builder;
using Nemeio.Core.Injection;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Builds;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.KeyboardFailures;
using Nemeio.Core.Keyboard.Monitors;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keys;
using Nemeio.Core.Keys.Executors;
using Nemeio.Core.Layouts;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Layouts.Import;
using Nemeio.Core.Layouts.LinkedApplications;
using Nemeio.Core.Layouts.Name;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.Layouts.Synchronization.Contexts;
using Nemeio.Core.Managers;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Models.SystemKeyboardCommand;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Strategies;
using Nemeio.Core.PackageUpdater.Tools;
using Nemeio.Core.PackageUpdater.Updatable.Factories;
using Nemeio.Core.Services;
using Nemeio.Core.Services.AppSettings;
using Nemeio.Core.Services.Blacklist;
using Nemeio.Core.Services.Category;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Settings;
using Nemeio.Core.Settings.Handlers.JpegCompressionLevel;
using Nemeio.Core.Settings.Parser;
using Nemeio.Core.Settings.Providers;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Hid;
using Nemeio.Core.Systems.Watchers;
using Nemeio.Core.Tools.Watchers;
using Nemeio.Infrastructure;
using Nemeio.Infrastructure.Repositories;
using Nemeio.Keyboard.Communication.Protocol.v1.Commands;
using Nemeio.Keyboard.Communication.Monitors;
using Nemeio.Presentation.Injection;
using Nemeio.Presentation.Menu;
using Nemeio.Presentation.PackageUpdater;
using Nemeio.Presentation.PackageUpdater.UI;
using Nemeio.Keyboard.Communication.Tools.Frames;
using Nemeio.Keyboard.Tools.Utils;
using Nemeio.Core.Layouts.Active.Requests.Factories;
using Nemeio.Core.Keyboard.Sessions;
using Nemeio.Core.Systems.Layouts;

namespace Nemeio.Presentation
{
    public class Application : MvxApplication
    {
        private static IDependencyRegister _dependencyRegister = new MvxDependencyRegister();

        public override void Initialize()
        {
            var loggerFactory = _dependencyRegister.Resolve<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Application>();

            logger.LogInformation("Application.Initialize");

            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            _dependencyRegister.LazyConstructAndRegisterSingleton<IFileSystem, FileSystem>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<IAugmentedLayoutImageProvider, AugmentedLayoutImageProvider>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<IErrorManager, ErrorManager>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<ILanguageManager, LanguageManager>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<INemeioHttpService, NemeioHttpService>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<IHttpService, HttpService>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<IDatabaseAccessFactory, DatabaseAccessFactory>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<IKeyboardErrorConverter, KeyboardErrorConverter>();

            ConnectDatabase();

            _dependencyRegister.ConstructAndRegisterSingleton<INetworkConnectivityChecker, NetworkConnectivityChecker>();
            _dependencyRegister.ConstructAndRegisterSingleton<ISystemKeyboardCommandHandler, SystemKeyboardCommandHandler>();
            _dependencyRegister.ConstructAndRegisterSingleton<IFontProvider, FontProvider>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<ILayoutValidityChecker, LayoutValidityChecker>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<ICategoryDbRepository, CategoryDbRepository>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<ILayoutDbRepository, LayoutDbRepository>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<IBlacklistDbRepository, BlacklistDbRepository>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<IApplicationSettingsDbRepository, ApplicationSettingsDbRepository>();

            //  Settings
            _dependencyRegister.LazyConstructAndRegisterSingleton<ISettingsParser, XmlSettingsParser>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<ISettingsProvider, SettingsProvider>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<ISettingsHolder, SettingsHolder>();

            //  Application
            _dependencyRegister.LazyConstructAndRegisterSingleton<IFirmwareManifestReader, FirmwareManifestReader>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<IApplicationSettingsProvider, ApplicationSettingsProvider>();
            _dependencyRegister.ConstructAndRegisterSingleton<IApplicationManifest, ApplicationManifest>();

            //  System
            _dependencyRegister.LazyConstructAndRegisterSingleton<ISystemLayoutWatcher, SystemLayoutWatcher>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<ISystemActiveLayoutWatcher, SystemActiveLayoutWatcher>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<ISystemHidInteractor, SystemHidInteractor>();
            _dependencyRegister.ConstructAndRegisterSingleton<ISystem, Nemeio.Core.Systems.System>();

            _dependencyRegister.ConstructAndRegisterSingleton<IJpegImagePackageBuilder, JpegImagePackageBuilder>();

            Nemeio.LayoutGen.Registerer.Register(_dependencyRegister);

            _dependencyRegister.ConstructAndRegisterSingleton<IScreenFactory, ScreenFactory>();

            //  Layouts
            _dependencyRegister.ConstructAndRegisterSingleton<ILayoutNameTransformer, LayoutNameTransformer>();
            _dependencyRegister.ConstructAndRegisterSingleton<ILayoutLibrary, LayoutLibrary>();
            _dependencyRegister.ConstructAndRegisterSingleton<ILayoutFactory, LayoutFactory>();
            _dependencyRegister.ConstructAndRegisterSingleton<ILayoutImporter, LayoutImporter>();
            _dependencyRegister.ConstructAndRegisterSingleton<ILayoutExportService, LayoutExportService>();
            _dependencyRegister.ConstructAndRegisterSingleton<IApplicationLayoutManager, ApplicationLayoutManager>();

            //  Active layout synchronization
            _dependencyRegister.ConstructAndRegisterSingleton<IChangeRequestFactory, ChangeRequestFactory>();
            _dependencyRegister.ConstructAndRegisterSingleton<IActiveLayoutSynchronizer, ActiveLayoutSynchronizer>();
            _dependencyRegister.ConstructAndRegisterSingleton<IActiveLayoutChangeHandler, ActiveLayoutChangeHandler>();
            _dependencyRegister.ConstructAndRegisterSingleton<INemeioLayoutEventStrategyFactory, NemeioLayoutEventStrategyFactory>();

            //  Keyboard and communication
            _dependencyRegister.ConstructAndRegisterSingleton<IFrameParser, FrameParser>();
            _dependencyRegister.ConstructAndRegisterSingleton<IKeyboardCommandFactory, KeyboardCommandFactory>();
            _dependencyRegister.ConstructAndRegisterSingleton<IMonitorFactory, MonitorFactory>();
            _dependencyRegister.ConstructAndRegisterSingleton<IKeyboardCrashLogger, ApplicationKeyboardCrashLogger>();
            _dependencyRegister.ConstructAndRegisterSingleton<INemeioFactory, NemeioFactory>();
            _dependencyRegister.ConstructAndRegisterSingleton<INemeioBuilder, NemeioBuilder>();
            _dependencyRegister.ConstructAndRegisterSingleton<IKeyboardCommandFactory, KeyboardCommandFactory>();
            _dependencyRegister.ConstructAndRegisterSingleton<IKeyboardSelector, KeyboardSelector>();
            _dependencyRegister.ConstructAndRegisterSingleton<IKeyboardProvider, KeyboardProvider>();
            _dependencyRegister.ConstructAndRegisterSingleton<IKeyboardController, KeyboardController>();

            _dependencyRegister.ConstructAndRegisterSingleton<ISystemLayoutEventHandler, SystemLayoutEventHandler>();

            //  Synchronization
            _dependencyRegister.ConstructAndRegisterSingleton<ISynchronizationContextFactory, SynchronizationContextFactory>();
            _dependencyRegister.ConstructAndRegisterSingleton<ISynchronizer, Synchronizer>();

            //  Keys
            _dependencyRegister.LazyConstructAndRegisterSingleton<IKeyExecutorFactory, KeyExecutorFactory>();
            _dependencyRegister.ConstructAndRegisterSingleton<IKeyHandler, KeyHandler>();

            _dependencyRegister.ConstructAndRegisterSingleton<ILayoutFacade, LayoutFacade>();

            _dependencyRegister.ConstructAndRegisterSingleton<IJpegCompressionLevelSettingHandler, JpegCompressionLevelSettingHandler>();

            //  Factory Reset
            _dependencyRegister.ConstructAndRegisterSingleton<IFactoryResetObserver, FactoryResetObserver>();

            //  Update
            _dependencyRegister.RegisterType<IInstallStrategyFactory, InstallStrategyFactory>();

            _dependencyRegister.ConstructAndRegisterSingleton<IPackageUpdateRepository, PackageUpdateRepository>();
            _dependencyRegister.ConstructAndRegisterSingleton<IPackageUpdaterFileProvider, PackageUpdaterFileProvider>();
            _dependencyRegister.ConstructAndRegisterSingleton<IPackageUpdateChecker, PackageUpdateChecker>();
            _dependencyRegister.ConstructAndRegisterSingleton<IUpdatableFactory, UpdatableFactory>();
            _dependencyRegister.ConstructAndRegisterSingleton<IPackageUpdaterTool, PackageUpdaterTool>();
            _dependencyRegister.ConstructAndRegisterSingleton<IPackageUpdater, Core.PackageUpdater.PackageUpdater>();
            _dependencyRegister.ConstructAndRegisterSingleton<IPackageUpdaterMessageFactory, PackageUpdaterMessageFactory>();

            _dependencyRegister.ConstructAndRegisterSingleton<IApplicationController, ApplicationController>();
        }

        public static void InitializeBeforeLastChance()
        {
            //  UIMenu
            _dependencyRegister.LazyConstructAndRegisterSingleton<IApplicationMenu, ApplicationMenu>();
            _dependencyRegister.LazyConstructAndRegisterSingleton<IUIMenu, UIMenu>();
        }

        public static void ApplicationStarted()
        {
            var loggerFactory = _dependencyRegister.Resolve<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Application>();

            var informationService = _dependencyRegister.Resolve<IInformationService>();

            logger.LogInformation("Application Started");
            logger.LogInformation($"- Application Version : {informationService.GetApplicationVersion()}");
            logger.LogInformation($"- Application Platform : {informationService.GetApplicationArchitecture()}");
            logger.LogInformation($"- System : {informationService.GetOperatingSystem()}");
            logger.LogInformation($"- System Platform : {informationService.GetSystemArchitecture()}");
            logger.LogInformation($"- Run on admin mode : {informationService.RunningOnAdministratorMode()}");

            PostInitializeApplication();
        }

        private void ConnectDatabase()
        {
            var loggerFactory = _dependencyRegister.Resolve<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Application>();

            try
            {
                var databaseAccessFactory = _dependencyRegister.Resolve<IDatabaseAccessFactory>();
                using (var dbAccess = databaseAccessFactory.CreateDatabaseAccess())
                {
                    dbAccess.DbContext.Database.Migrate();
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                logger.LogError(exception, $"Can't write password");

                var dialogService = _dependencyRegister.Resolve<IDialogService>();
                dialogService.ShowMessage(
                    StringId.ApplicationTitleName,
                    StringId.KeychainErrorMessageUserUnauthorized
                );

                throw;
            }
        }

        private static void PostInitializeApplication()
        {
            var applicationSettingsManager = _dependencyRegister.Resolve<IApplicationSettingsProvider>();

            _dependencyRegister.Resolve<ILanguageManager>().InjectApplicationSettingsManager(applicationSettingsManager);
            _dependencyRegister.Resolve<IApplicationController>().Start();
        }
    }
}
