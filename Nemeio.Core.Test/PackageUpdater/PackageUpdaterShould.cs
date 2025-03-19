using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Applications;
using Nemeio.Core.Connectivity;
using Nemeio.Core.DataModels;
using Nemeio.Core.Errors;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Updates;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.PackageUpdater.Tools;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Core.PackageUpdater.Updatable.Factories;
using Nemeio.Core.PackageUpdater.Updatable.States;
using Nemeio.Core.Services;
using Nemeio.Core.Services.AppSettings;
using Nemeio.Core.Tools.StateMachine;
using NUnit.Framework;

namespace Nemeio.Core.Test.PackageUpdater
{
    public class PackageUpdaterShould
    {
        [Test]
        public void PackageUpdater_Constructor_Ok()
        {
            var mockLoggerFactory = Mock.Of<ILoggerFactory>();
            var mockErrorManager = Mock.Of<IErrorManager>();
            var mockPackageChecker = Mock.Of<IPackageUpdateChecker>();
            var mockPackageFileProvider = Mock.Of<IPackageUpdaterFileProvider>();
            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            var mockDocument = Mock.Of<IDocument>();
            var mockFileSystem = Mock.Of<IFileSystem>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();
            var mockUpdatableFactory = Mock.Of<IUpdatableFactory>();
            var mockPackageUpdaterTool = Mock.Of<IPackageUpdaterTool>();
            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();

            Assert.DoesNotThrow(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(null, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, null, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, null, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, null, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, null, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, null, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, null, mockNetworkConnectivityChecker, mockUpdatableFactory, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, null, mockUpdatableFactory, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, null, mockPackageUpdaterTool, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, null, mockApplicationSettingsManager));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, mockPackageUpdaterTool, null));
            Assert.Throws<ArgumentNullException>(() => new Core.PackageUpdater.PackageUpdater(null, null, null, null, null, null, null, null, null, null, null));
        }

        [Test]
        public async Task PackageUpdater_State_Idle_ChangeTo_CheckApplicationUpdate_WhenCheckUpdatesAsync_AndInternetAvailable_AndApplicationUpdateFound()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var mockLoggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();

            var mockPackageChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .ReturnsAsync(new DownloadablePackageInformation(new Version("2.0.0"), "21", new Uri("http://www.google.fr")));

            var mockPackageFileProvider = Mock.Of<IPackageUpdaterFileProvider>();
            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(true);

            var mockDocument = Mock.Of<IDocument>();
            var mockFileSystem = Mock.Of<IFileSystem>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();

            var mockUpdatableSoftware = Mock.Of<IUpdatable>();

            var mockUpdatableFactory = Mock.Of<IUpdatableFactory>();
            Mock.Get(mockUpdatableFactory)
                .Setup(x => x.CreateUpdatableSoftware(It.IsAny<DownloadablePackageInformation>()))
                .Returns(mockUpdatableSoftware);

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            Mock.Get(mockApplicationSettingsManager)
                .Setup(x => x.ApplicationSettings)
                .Returns(new ApplicationSettings(new CultureInfo("fr-FR"), false, false, null, string.Empty));

            var mockInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy("1.0.0"));

            var packageUpdaterTool = new PackageUpdaterTool(mockLoggerFactory, mockApplicationSettingsManager, mockInformationService);

            var stateHistoric = new List<PackageUpdateState>();

            var packageUpdater = new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, packageUpdaterTool, mockApplicationSettingsManager);
            packageUpdater.OnStateChanged += (object sender, OnStateChangedEventArgs<PackageUpdateState> eventArgs) =>
            {
                stateHistoric.Add(eventArgs.State);
            };
            packageUpdater.OnUpdateAvailable += delegate
            {
                stateHistoric.Count.Should().Be(3);
                stateHistoric[0].Should().Be(PackageUpdateState.CheckApplicationInstallation);
                stateHistoric[1].Should().Be(PackageUpdateState.CheckInternetConnection);
                stateHistoric[2].Should().Be(PackageUpdateState.CheckApplicationUpdate);

                try
                {
                    semaphore.Release();
                }
                catch (Exception) { }
            };

            packageUpdater.State.Should().Be(PackageUpdateState.Idle);

            await packageUpdater.CheckUpdatesAsync();

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }
        }

        [Test]
        public async Task PackageUpdater_State_Idle_ChangeTo_Idle_WhenCheckUpdatesAsync_AndInternetNotAvailable()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var mockLoggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();

            var mockPackageChecker = Mock.Of<IPackageUpdateChecker>();
            var mockPackageFileProvider = Mock.Of<IPackageUpdaterFileProvider>();
            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(false);

            var mockDocument = Mock.Of<IDocument>();
            var mockFileSystem = Mock.Of<IFileSystem>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();

            var mockUpdatableSoftware = Mock.Of<IUpdatable>();

            var mockUpdatableFactory = Mock.Of<IUpdatableFactory>();
            Mock.Get(mockUpdatableFactory)
                .Setup(x => x.CreateUpdatableSoftware(It.IsAny<DownloadablePackageInformation>()))
                .Returns(mockUpdatableSoftware);

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            Mock.Get(mockApplicationSettingsManager)
                .Setup(x => x.ApplicationSettings)
                .Returns(new ApplicationSettings(new CultureInfo("fr-FR"), false, false, null, string.Empty));

            var mockInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy("1.0.0"));

            var packageUpdaterTool = new PackageUpdaterTool(mockLoggerFactory, mockApplicationSettingsManager, mockInformationService);

            var stateHistoric = new List<PackageUpdateState>();

            var packageUpdater = new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, packageUpdaterTool, mockApplicationSettingsManager);
            packageUpdater.OnStateChanged += (object sender, OnStateChangedEventArgs<PackageUpdateState> eventArgs) =>
            {
                stateHistoric.Add(eventArgs.State);

                if (eventArgs.State == PackageUpdateState.Idle)
                {
                    try
                    {
                        semaphore.Release();
                    }
                    catch (Exception) { }
                }
            };

            packageUpdater.State.Should().Be(PackageUpdateState.Idle);

            await packageUpdater.CheckUpdatesAsync();

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }

            stateHistoric.Count.Should().Be(3);
            stateHistoric[0].Should().Be(PackageUpdateState.CheckApplicationInstallation);
            stateHistoric[1].Should().Be(PackageUpdateState.CheckInternetConnection);
            stateHistoric[2].Should().Be(PackageUpdateState.Idle);
        }

        [Test]
        public async Task PackageUpdater_State_Idle_ChangeTo_Idle_WhenCheckUpdatesAsync_AndInternetAvailable_ButNoApplicationUpdate_AndNoKeyboardPlugged()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var mockLoggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();

            DownloadablePackageInformation downloadablePackage = null;
            INemeio nemeio = null;

            var mockPackageChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .ReturnsAsync(downloadablePackage);

            var mockPackageFileProvider = Mock.Of<IPackageUpdaterFileProvider>();
            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(true);

            var mockDocument = Mock.Of<IDocument>();
            var mockFileSystem = Mock.Of<IFileSystem>();

            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(false);
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Nemeio)
                .Returns(nemeio);

            var mockUpdatableSoftware = Mock.Of<IUpdatable>();

            var mockUpdatableFactory = Mock.Of<IUpdatableFactory>();
            Mock.Get(mockUpdatableFactory)
                .Setup(x => x.CreateUpdatableSoftware(It.IsAny<DownloadablePackageInformation>()))
                .Returns(mockUpdatableSoftware);

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            Mock.Get(mockApplicationSettingsManager)
                .Setup(x => x.ApplicationSettings)
                .Returns(new ApplicationSettings(new CultureInfo("fr-FR"), false, false, null, string.Empty));

            var mockInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy("1.0.0"));

            var packageUpdaterTool = new PackageUpdaterTool(mockLoggerFactory, mockApplicationSettingsManager, mockInformationService);

            var stateHistoric = new List<PackageUpdateState>();

            var packageUpdater = new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, packageUpdaterTool, mockApplicationSettingsManager);
            packageUpdater.OnStateChanged += (object sender, OnStateChangedEventArgs<PackageUpdateState> eventArgs) =>
            {
                stateHistoric.Add(eventArgs.State);

                if (eventArgs.State == PackageUpdateState.Idle)
                {
                    try
                    {
                        semaphore.Release();
                    }
                    catch (Exception) { }
                }
            };

            packageUpdater.State.Should().Be(PackageUpdateState.Idle);

            await packageUpdater.CheckUpdatesAsync();

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }

            stateHistoric.Count.Should().Be(5);
            stateHistoric[0].Should().Be(PackageUpdateState.CheckApplicationInstallation);
            stateHistoric[1].Should().Be(PackageUpdateState.CheckInternetConnection);
            stateHistoric[2].Should().Be(PackageUpdateState.CheckApplicationUpdate);
            stateHistoric[3].Should().Be(PackageUpdateState.CheckFirmwareUpdate);
            stateHistoric[4].Should().Be(PackageUpdateState.Idle);
        }

        public interface ITestUpdateNemeio : INemeio, IUpdateHolder { }

        [Test]
        public async Task PackageUpdater_State_Idle_ChangeTo_Idle_WhenCheckUpdatesAsync_AndInternetAvailable_ButNoApplicationUpdate_AndKeyboardPlugged_ButKeyboardIsUpToDate()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var mockLoggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();

            DownloadablePackageInformation downloadablePackage = null;

            var mockPackageChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .ReturnsAsync(downloadablePackage);

            var mockPackageFileProvider = Mock.Of<IPackageUpdaterFileProvider>();
            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(true);

            var mockDocument = Mock.Of<IDocument>();
            var mockFileSystem = Mock.Of<IFileSystem>();

            var nemeio = Mock.Of<INemeio>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(true);
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Nemeio)
                .Returns(nemeio);

            var mockUpdatableFactory = Mock.Of<IUpdatableFactory>();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            Mock.Get(mockApplicationSettingsManager)
                .Setup(x => x.ApplicationSettings)
                .Returns(new ApplicationSettings(new CultureInfo("fr-FR"), false, false, null, string.Empty));

            var mockInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy("1.0.0"));

            var packageUpdaterTool = new PackageUpdaterTool(mockLoggerFactory, mockApplicationSettingsManager, mockInformationService);

            var stateHistoric = new List<PackageUpdateState>();

            var packageUpdater = new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, packageUpdaterTool, mockApplicationSettingsManager);
            packageUpdater.OnStateChanged += (object sender, OnStateChangedEventArgs<PackageUpdateState> eventArgs) =>
            {
                stateHistoric.Add(eventArgs.State);

                if (eventArgs.State == PackageUpdateState.Idle)
                {
                    try
                    {
                        semaphore.Release();
                    }
                    catch (Exception) { }
                }
            };

            packageUpdater.State.Should().Be(PackageUpdateState.Idle);

            await packageUpdater.CheckUpdatesAsync();

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }

            stateHistoric.Count.Should().Be(5);
            stateHistoric[0].Should().Be(PackageUpdateState.CheckApplicationInstallation);
            stateHistoric[1].Should().Be(PackageUpdateState.CheckInternetConnection);
            stateHistoric[2].Should().Be(PackageUpdateState.CheckApplicationUpdate);
            stateHistoric[3].Should().Be(PackageUpdateState.CheckFirmwareUpdate);
            stateHistoric[4].Should().Be(PackageUpdateState.Idle);
        }

        [Test]
        public async Task PackageUpdater_State_Idle_ChangeTo_UpdatePending_WhenCheckUpdatesAsync_AndInternetAvailable_ButNoApplicationUpdate_AndKeyboardPlugged_AndKeyboardUpdateIsAvailable()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var mockLoggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();

            DownloadablePackageInformation downloadablePackage = null;

            var mockPackageChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .ReturnsAsync(downloadablePackage);

            var mockPackageFileProvider = Mock.Of<IPackageUpdaterFileProvider>();
            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(true);

            var mockDocument = Mock.Of<IDocument>();
            var mockFileSystem = Mock.Of<IFileSystem>();

            var nemeio = Mock.Of<ITestUpdateNemeio>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(true);
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Nemeio)
                .Returns(nemeio);

            var mockUpdatable = Mock.Of<IUpdatable>();
            var mockUpdatableFactory = Mock.Of<IUpdatableFactory>();
            Mock.Get(mockUpdatableFactory)
                .Setup(x => x.CreateUpdatableKeyboard(It.IsAny<FirmwareUpdatableNemeioProxy>()))
                .Returns(mockUpdatable);

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            Mock.Get(mockApplicationSettingsManager)
                .Setup(x => x.ApplicationSettings)
                .Returns(new ApplicationSettings(new CultureInfo("fr-FR"), false, false, null, string.Empty));

            var mockInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy("1.0.0"));

            var packageUpdaterTool = new PackageUpdaterTool(mockLoggerFactory, mockApplicationSettingsManager, mockInformationService);

            var stateHistoric = new List<PackageUpdateState>();

            var packageUpdater = new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, packageUpdaterTool, mockApplicationSettingsManager);
            packageUpdater.OnStateChanged += (object sender, OnStateChangedEventArgs<PackageUpdateState> eventArgs) =>
            {
                stateHistoric.Add(eventArgs.State);

                if (eventArgs.State == PackageUpdateState.UpdatePending)
                {
                    try
                    {
                        semaphore.Release();
                    }
                    catch (Exception) { }
                }
            };

            packageUpdater.State.Should().Be(PackageUpdateState.Idle);

            await packageUpdater.CheckUpdatesAsync();

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }

            stateHistoric.Count.Should().Be(5);
            stateHistoric[0].Should().Be(PackageUpdateState.CheckApplicationInstallation);
            stateHistoric[1].Should().Be(PackageUpdateState.CheckInternetConnection);
            stateHistoric[2].Should().Be(PackageUpdateState.CheckApplicationUpdate);
            stateHistoric[3].Should().Be(PackageUpdateState.CheckFirmwareUpdate);
            stateHistoric[4].Should().Be(PackageUpdateState.UpdatePending);
        }

        [Test]
        public async Task PackageUpdater_State_Idle_To_Updating_WhenKeyboardHasUpdate_AndUserStartUpdating()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var mockLoggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();

            DownloadablePackageInformation downloadablePackage = null;

            var mockPackageChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .ReturnsAsync(downloadablePackage);

            var mockPackageFileProvider = Mock.Of<IPackageUpdaterFileProvider>();
            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(true);

            var mockDocument = Mock.Of<IDocument>();
            var mockFileSystem = Mock.Of<IFileSystem>();

            var nemeio = Mock.Of<ITestUpdateNemeio>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(true);
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Nemeio)
                .Returns(nemeio);

            var mockUpdatable = Mock.Of<IUpdatable>();
            var mockUpdatableFactory = Mock.Of<IUpdatableFactory>();
            Mock.Get(mockUpdatableFactory)
                .Setup(x => x.CreateUpdatableKeyboard(It.IsAny<FirmwareUpdatableNemeioProxy>()))
                .Returns(mockUpdatable);

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            Mock.Get(mockApplicationSettingsManager)
                .Setup(x => x.ApplicationSettings)
                .Returns(new ApplicationSettings(new CultureInfo("fr-FR"), false, false, null, string.Empty));

            var mockInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy("1.0.0"));

            var packageUpdaterTool = new PackageUpdaterTool(mockLoggerFactory, mockApplicationSettingsManager, mockInformationService);

            var stateHistoric = new List<PackageUpdateState>();

            var packageUpdater = new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, packageUpdaterTool, mockApplicationSettingsManager);
            packageUpdater.OnStateChanged += async (object sender, OnStateChangedEventArgs<PackageUpdateState> eventArgs) =>
            {
                stateHistoric.Add(eventArgs.State);

                if (eventArgs.State == PackageUpdateState.UpdatePending)
                {
                    stateHistoric.Count.Should().Be(5);
                    stateHistoric[0].Should().Be(PackageUpdateState.CheckApplicationInstallation);
                    stateHistoric[1].Should().Be(PackageUpdateState.CheckInternetConnection);
                    stateHistoric[2].Should().Be(PackageUpdateState.CheckApplicationUpdate);
                    stateHistoric[3].Should().Be(PackageUpdateState.CheckFirmwareUpdate);
                    stateHistoric[4].Should().Be(PackageUpdateState.UpdatePending);

                    await packageUpdater.InstallUpdateAsync();
                }
                else if (eventArgs.State == PackageUpdateState.ApplyUpdate)
                {
                    stateHistoric.Count.Should().Be(6);
                    stateHistoric[5].Should().Be(PackageUpdateState.ApplyUpdate);

                    try
                    {
                        semaphore.Release();
                    }
                    catch (Exception) { }
                }
            };

            packageUpdater.State.Should().Be(PackageUpdateState.Idle);

            await packageUpdater.CheckUpdatesAsync();

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }
        }

        public static Array UpdateComponents() => Enum.GetValues(typeof(UpdateComponent));

        [TestCaseSource("UpdateComponents")]
        public async Task PackageUpdater_State_Idle_To_Updating_WhenKeyboardHasUpdate_AndUserStartUpdating_ButUpdateFailed(UpdateComponent component)
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var mockLoggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();

            DownloadablePackageInformation downloadablePackage = null;

            var mockPackageChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .ReturnsAsync(downloadablePackage);

            var mockPackageFileProvider = Mock.Of<IPackageUpdaterFileProvider>();
            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(true);

            var mockDocument = Mock.Of<IDocument>();
            var mockFileSystem = Mock.Of<IFileSystem>();

            var nemeio = Mock.Of<ITestUpdateNemeio>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(true);
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Nemeio)
                .Returns(nemeio);

            var mockUpdatable = Mock.Of<IUpdatable>();
            var mockUpdatableFactory = Mock.Of<IUpdatableFactory>();
            Mock.Get(mockUpdatableFactory)
                .Setup(x => x.CreateUpdatableKeyboard(It.IsAny<FirmwareUpdatableNemeioProxy>()))
                .Returns(mockUpdatable);

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            Mock.Get(mockApplicationSettingsManager)
                .Setup(x => x.ApplicationSettings)
                .Returns(new ApplicationSettings(new CultureInfo("fr-FR"), false, false, null, string.Empty));

            var mockInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy("1.0.0"));

            var packageUpdaterTool = new PackageUpdaterTool(mockLoggerFactory, mockApplicationSettingsManager, mockInformationService);

            var stateHistoric = new List<PackageUpdateState>();

            var packageUpdater = new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, packageUpdaterTool, mockApplicationSettingsManager);
            packageUpdater.OnStateChanged += async (object sender, OnStateChangedEventArgs<PackageUpdateState> eventArgs) =>
            {
                stateHistoric.Add(eventArgs.State);

                if (eventArgs.State == PackageUpdateState.UpdatePending)
                {
                    stateHistoric.Count.Should().Be(5);
                    stateHistoric[0].Should().Be(PackageUpdateState.CheckApplicationInstallation);
                    stateHistoric[1].Should().Be(PackageUpdateState.CheckInternetConnection);
                    stateHistoric[2].Should().Be(PackageUpdateState.CheckApplicationUpdate);
                    stateHistoric[3].Should().Be(PackageUpdateState.CheckFirmwareUpdate);
                    stateHistoric[4].Should().Be(PackageUpdateState.UpdatePending);

                    await packageUpdater.InstallUpdateAsync();
                }
                else if (eventArgs.State == PackageUpdateState.ApplyUpdate)
                {
                    stateHistoric.Count.Should().Be(6);
                    stateHistoric[5].Should().Be(PackageUpdateState.ApplyUpdate);

                    var _ = Task.Run(() =>
                    {
                        Mock.Get(mockUpdatable)
                            .Raise(x => x.OnUpdateFinished += null, mockUpdatable, new UpdateFinishedEventArgs(ErrorCode.AclKeyboardResponseFirmwareUpdateFailed));
                    });
                }
                else if (eventArgs.State == PackageUpdateState.UpdateFailed)
                {
                    stateHistoric.Count.Should().Be(7);
                    stateHistoric[6].Should().Be(PackageUpdateState.UpdateFailed);

                    try
                    {
                        semaphore.Release();
                    }
                    catch (Exception) { }
                }
            };

            packageUpdater.State.Should().Be(PackageUpdateState.Idle);

            await packageUpdater.CheckUpdatesAsync();

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }
        }

        [Test]
        public async Task PackageUpdater_State_Idle_To_Updating_WhenKeyboardHasUpdate_AndUserStartUpdating_ButUpdateSucceed()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var mockLoggerFactory = new LoggerFactory();
            var mockErrorManager = Mock.Of<IErrorManager>();

            DownloadablePackageInformation downloadablePackage = null;

            var mockPackageChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .ReturnsAsync(downloadablePackage);

            var mockPackageFileProvider = Mock.Of<IPackageUpdaterFileProvider>();
            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(true);

            var mockDocument = Mock.Of<IDocument>();
            var mockFileSystem = Mock.Of<IFileSystem>();

            var nemeio = Mock.Of<ITestUpdateNemeio>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(true);
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Nemeio)
                .Returns(nemeio);

            var mockUpdatable = Mock.Of<IUpdatable>();
            var mockUpdatableFactory = Mock.Of<IUpdatableFactory>();
            Mock.Get(mockUpdatableFactory)
                .Setup(x => x.CreateUpdatableKeyboard(It.IsAny<FirmwareUpdatableNemeioProxy>()))
                .Returns(mockUpdatable);

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            Mock.Get(mockApplicationSettingsManager)
                .Setup(x => x.ApplicationSettings)
                .Returns(new ApplicationSettings(new CultureInfo("fr-FR"), false, false, null, string.Empty));

            var mockInformationService = Mock.Of<IInformationService>();
            Mock.Get(mockInformationService)
                .Setup(x => x.GetApplicationVersion())
                .Returns(new VersionProxy("1.0.0"));

            var packageUpdaterTool = new PackageUpdaterTool(mockLoggerFactory, mockApplicationSettingsManager, mockInformationService);

            var stateHistoric = new List<PackageUpdateState>();

            var packageUpdater = new Core.PackageUpdater.PackageUpdater(mockLoggerFactory, mockDocument, mockFileSystem, mockErrorManager, mockPackageChecker, mockPackageFileProvider, mockKeyboardController, mockNetworkConnectivityChecker, mockUpdatableFactory, packageUpdaterTool, mockApplicationSettingsManager);
            packageUpdater.OnStateChanged += async (object sender, OnStateChangedEventArgs<PackageUpdateState> eventArgs) =>
            {
                stateHistoric.Add(eventArgs.State);

                if (eventArgs.State == PackageUpdateState.UpdatePending)
                {
                    stateHistoric.Count.Should().Be(5);
                    stateHistoric[0].Should().Be(PackageUpdateState.CheckApplicationInstallation);
                    stateHistoric[1].Should().Be(PackageUpdateState.CheckInternetConnection);
                    stateHistoric[2].Should().Be(PackageUpdateState.CheckApplicationUpdate);
                    stateHistoric[3].Should().Be(PackageUpdateState.CheckFirmwareUpdate);
                    stateHistoric[4].Should().Be(PackageUpdateState.UpdatePending);

                    await packageUpdater.InstallUpdateAsync();
                }
                else if (eventArgs.State == PackageUpdateState.ApplyUpdate)
                {
                    stateHistoric.Count.Should().Be(6);
                    stateHistoric[5].Should().Be(PackageUpdateState.ApplyUpdate);

                    var _ = Task.Run(() =>
                    {
                        Mock.Get(mockUpdatable)
                            .Raise(x => x.OnUpdateFinished += null, mockUpdatable, new UpdateFinishedEventArgs(ErrorCode.Success));
                    });
                }
                else if (eventArgs.State == PackageUpdateState.UpdateSucceed)
                {
                    stateHistoric.Count.Should().Be(7);
                    stateHistoric[6].Should().Be(PackageUpdateState.UpdateSucceed);

                    try
                    {
                        semaphore.Release();
                    }
                    catch (Exception) { }
                }
            };

            packageUpdater.State.Should().Be(PackageUpdateState.Idle);

            await packageUpdater.CheckUpdatesAsync();

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }
        }
    }
}
