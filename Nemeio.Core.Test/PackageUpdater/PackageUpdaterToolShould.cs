using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Applications;
using Nemeio.Core.Connectivity;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Nemeios;
using Nemeio.Core.Keyboard.Updates;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Downloader;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.PackageUpdater.Tools;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Core.PackageUpdater.Updatable.States;
using Nemeio.Core.Services;
using NUnit.Framework;

namespace Nemeio.Core.Test.PackageUpdater
{
    [TestFixture]
    public class PackageUpdaterToolShould
    {
        [Test]
        public void PackageUpdaterTool_DownloadDependenciesAsync_WhenParametersAreInvalid_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            var mockDownloader = Mock.Of<IPackagesDownloader>();
            var mockUpdatable = Mock.Of<IUpdatable>();

            Assert.ThrowsAsync<ArgumentNullException>(() => tool.DownloadDependenciesAsync(null, mockDownloader));
            Assert.ThrowsAsync<ArgumentNullException>(() => tool.DownloadDependenciesAsync(mockUpdatable, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => tool.DownloadDependenciesAsync(null, null));
        }

        [Test]
        public async Task PackageUpdaterTool_DownloadDependenciesAsync_WhenDownloadFail_RaiseEventWithSuccessEqualsFalse()
        {
            var onDownloadFinishedCalled = false;
            var downloadIsSuccess = false;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnDownloadFinished += (object sender, DownloadFinishedEventArgs eventArgs) => 
            {
                onDownloadFinishedCalled = true;
                downloadIsSuccess = eventArgs.IsSuccess;
            };

            var mockUpdatable = Mock.Of<IUpdatable>();
            Mock.Get(mockUpdatable)
                .Setup(x => x.DownloadDependenciesAsync(It.IsAny<IPackagesDownloader>()))
                .Throws(new InvalidOperationException("This is a unit tests error"));

            var mockDownloader = Mock.Of<IPackagesDownloader>();

            await tool.DownloadDependenciesAsync(mockUpdatable, mockDownloader);

            onDownloadFinishedCalled.Should().BeTrue();
            downloadIsSuccess.Should().BeFalse();
        }

        [Test]
        public async Task PackageUpdaterTool_DownloadDependenciesAsync_WhenDownloadSucceed_RaiseEventWithSuccessEqualsTrue()
        {
            var testSemaphore = new SemaphoreSlim(1, 1);
            var onDownloadFinishedCalled = false;
            var downloadIsSuccess = false;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnDownloadFinished += (object sender, DownloadFinishedEventArgs eventArgs) =>
            {
                onDownloadFinishedCalled = true;
                downloadIsSuccess = eventArgs.IsSuccess;
            };

            var mockUpdatable = Mock.Of<IUpdatable>();
            Mock.Get(mockUpdatable)
                .Setup(x => x.DownloadDependenciesAsync(It.IsAny<IPackagesDownloader>()))
                .Returns(Task.Delay(0));

            var mockDownloader = Mock.Of<IPackagesDownloader>();

            await tool.DownloadDependenciesAsync(mockUpdatable, mockDownloader);

            onDownloadFinishedCalled.Should().BeTrue();
            downloadIsSuccess.Should().BeTrue();
        }

        [Test]
        public void PackageUpdaterTool_CheckInternetConnectionAsync_WhenParametersAreInvalid_Throws()
        {
            var loggerFactory = new LoggerFactory();
            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);

            Assert.ThrowsAsync<ArgumentNullException>(() => tool.CheckInternetConnectionAsync(null));
        }

        [Test]
        public async Task PackageUpdaterTool_CheckInternetConnectionAsync_WhenInternetIsAvailable_RaiseEventWithAvailableEqualsTrue()
        {
            var onInternetStateComputedCalled = false;
            var internetIsAvailable = false;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnInternetStateComputed += (object sender, InternetIsAvailableEventArgs eventArgs) =>
            {
                onInternetStateComputedCalled = true;
                internetIsAvailable = eventArgs.IsAvailable;
            };

            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(true);

            await tool.CheckInternetConnectionAsync(mockNetworkConnectivityChecker);

            onInternetStateComputedCalled.Should().BeTrue();
            internetIsAvailable.Should().BeTrue();
        }

        [Test]
        public async Task PackageUpdaterTool_CheckInternetConnectionAsync_WhenInternetIsNotAvailable_RaiseEventWithAvailableEqualsFalse()
        {
            var onInternetStateComputedCalled = false;
            var internetIsAvailable = false;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnInternetStateComputed += (object sender, InternetIsAvailableEventArgs eventArgs) =>
            {
                onInternetStateComputedCalled = true;
                internetIsAvailable = eventArgs.IsAvailable;
            };

            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .ReturnsAsync(false);

            await tool.CheckInternetConnectionAsync(mockNetworkConnectivityChecker);

            onInternetStateComputedCalled.Should().BeTrue();
            internetIsAvailable.Should().BeFalse();
        }

        [Test]
        public async Task PackageUpdaterTool_CheckInternetConnectionAsync_WhenErrorOccurWhenCheck_RaiseEventWithAvailableEqualsFalse()
        {
            var onInternetStateComputedCalled = false;
            var internetIsAvailable = false;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnInternetStateComputed += (object sender, InternetIsAvailableEventArgs eventArgs) =>
            {
                onInternetStateComputedCalled = true;
                internetIsAvailable = eventArgs.IsAvailable;
            };

            var mockNetworkConnectivityChecker = Mock.Of<INetworkConnectivityChecker>();
            Mock.Get(mockNetworkConnectivityChecker)
                .Setup(x => x.InternetIsAvailable())
                .Throws(new InvalidOperationException("This is a unit tests error"));

            await tool.CheckInternetConnectionAsync(mockNetworkConnectivityChecker);

            onInternetStateComputedCalled.Should().BeTrue();
            internetIsAvailable.Should().BeFalse();
        }

        [Test]
        public void PackageUpdaterTool_CheckApplicationUpdateAsync_WhenParametersAreInvalid_Throws()
        {
            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);

            Assert.ThrowsAsync<ArgumentNullException>(() => tool.CheckApplicationUpdateAsync(null));
        }

        [Test]
        public async Task PackageUpdaterTool_CheckApplicationUpdateAsync_WhenNoUpdate_RaiseEvent()
        {
            var onApplicationUpdateCheckedCalled = false;
            var updateFound = false;
            IList<DownloadablePackageInformation> packages = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnApplicationUpdateChecked += (object sender, ApplicationUpdateCheckedEventArgs eventArgs) =>
            {
                onApplicationUpdateCheckedCalled = true;
                updateFound = eventArgs.Found;
                packages = eventArgs.Packages;
            };

            DownloadablePackageInformation update = null;

            var mockPackageUpdateChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageUpdateChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .ReturnsAsync(update);

            await tool.CheckApplicationUpdateAsync(mockPackageUpdateChecker);

            onApplicationUpdateCheckedCalled.Should().BeTrue();
            updateFound.Should().BeFalse();
            packages.Should().NotBeNull();
            packages.Should().BeEmpty();
        }

        [Test]
        public async Task PackageUpdaterTool_CheckApplicationUpdateAsync_WhenErrorOccur_RaiseEvent()
        {
            var onApplicationUpdateCheckedCalled = false;
            var updateFound = false;
            IList<DownloadablePackageInformation> packages = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnApplicationUpdateChecked += (object sender, ApplicationUpdateCheckedEventArgs eventArgs) =>
            {
                onApplicationUpdateCheckedCalled = true;
                updateFound = eventArgs.Found;
                packages = eventArgs.Packages;
            };

            var mockPackageUpdateChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageUpdateChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .Throws(new InvalidOperationException("This is a unit tests error"));

            await tool.CheckApplicationUpdateAsync(mockPackageUpdateChecker);

            onApplicationUpdateCheckedCalled.Should().BeTrue();
            updateFound.Should().BeFalse();
            packages.Should().NotBeNull();
            packages.Should().BeEmpty();
        }

        [Test]
        public async Task PackageUpdaterTool_CheckApplicationUpdateAsync_WhenUpdateFound_RaiseEvent()
        {
            var onApplicationUpdateCheckedCalled = false;
            var updateFound = false;
            IList<DownloadablePackageInformation> packages = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnApplicationUpdateChecked += (object sender, ApplicationUpdateCheckedEventArgs eventArgs) =>
            {
                onApplicationUpdateCheckedCalled = true;
                updateFound = eventArgs.Found;
                packages = eventArgs.Packages;
            };

            var update = new DownloadablePackageInformation(
                new Version("1.0.0"),
                "XXX",
                new Uri("http://www.google.fr")
            );

            var mockPackageUpdateChecker = Mock.Of<IPackageUpdateChecker>();
            Mock.Get(mockPackageUpdateChecker)
                .Setup(x => x.ApplicationNeedUpdateAsync())
                .ReturnsAsync(update);

            await tool.CheckApplicationUpdateAsync(mockPackageUpdateChecker);

            onApplicationUpdateCheckedCalled.Should().BeTrue();
            updateFound.Should().BeTrue();
            packages.Should().NotBeNull();
            packages.Should().NotBeEmpty();
            packages.Count.Should().Be(1);
        }

        [Test]
        public void PackageUpdaterTool_CheckFirmwareUpdateAsync_WhenParametersAreInvalid_Throws()
        {
            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);

            Assert.ThrowsAsync<ArgumentNullException>(() => tool.CheckFirmwareUpdateAsync(null));
        }

        [Test]
        public async Task PackageUpdaterTool_CheckFirmwareUpdateAsync_WhenErrorOccured_RaiseEvent()
        {
            var onFirmwareUpdateCheckedCalled = false;
            var updateFound = false;
            FirmwareUpdatableNemeioProxy proxy = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnFirmwareUpdateChecked += (object sender, FirmwareUpdateCheckedEventArgs eventArgs) =>
            {
                onFirmwareUpdateCheckedCalled = true;
                updateFound = eventArgs.Found;
                proxy = eventArgs.Proxy;
            };

            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Throws(new InvalidOperationException("This is a unit tests error"));

            await tool.CheckFirmwareUpdateAsync(mockKeyboardController);

            onFirmwareUpdateCheckedCalled.Should().BeTrue();
            updateFound.Should().BeFalse();
            proxy.Should().BeNull();
        }

        [Test]
        public async Task PackageUpdaterTool_CheckFirmwareUpdateAsync_WhenKeyboardIsNotConnected_RaiseEvent()
        {
            var onFirmwareUpdateCheckedCalled = false;
            var updateFound = false;
            FirmwareUpdatableNemeioProxy proxy = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnFirmwareUpdateChecked += (object sender, FirmwareUpdateCheckedEventArgs eventArgs) =>
            {
                onFirmwareUpdateCheckedCalled = true;
                updateFound = eventArgs.Found;
                proxy = eventArgs.Proxy;
            };

            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(false);

            await tool.CheckFirmwareUpdateAsync(mockKeyboardController);

            onFirmwareUpdateCheckedCalled.Should().BeTrue();
            updateFound.Should().BeFalse();
            proxy.Should().BeNull();
        }

        public interface IRunnerNemeio : INemeio { }

        [Test]
        public async Task PackageUpdaterTool_CheckFirmwareUpdateAsync_WhenKeyboardIsConnected_ButNotUpdatable_RaiseEvent()
        {
            var onFirmwareUpdateCheckedCalled = false;
            var updateFound = false;
            FirmwareUpdatableNemeioProxy proxy = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnFirmwareUpdateChecked += (object sender, FirmwareUpdateCheckedEventArgs eventArgs) =>
            {
                onFirmwareUpdateCheckedCalled = true;
                updateFound = eventArgs.Found;
                proxy = eventArgs.Proxy;
            };

            var mockNemeio = Mock.Of<IRunnerNemeio>();

            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(true);
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Nemeio)
                .Returns(mockNemeio);

            await tool.CheckFirmwareUpdateAsync(mockKeyboardController);

            onFirmwareUpdateCheckedCalled.Should().BeTrue();
            updateFound.Should().BeFalse();
            proxy.Should().BeNull();
        }

        public interface IUpdatableNemeio : INemeio, IUpdateHolder { }

        [Test]
        public async Task PackageUpdaterTool_CheckFirmwareUpdateAsync_WhenKeyboardIsConnected_AndUpdatable_RaiseEvent()
        {
            var onFirmwareUpdateCheckedCalled = false;
            var updateFound = false;
            FirmwareUpdatableNemeioProxy proxy = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnFirmwareUpdateChecked += (object sender, FirmwareUpdateCheckedEventArgs eventArgs) =>
            {
                onFirmwareUpdateCheckedCalled = true;
                updateFound = eventArgs.Found;
                proxy = eventArgs.Proxy;
            };

            var mockNemeio = Mock.Of<IUpdatableNemeio>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Connected)
                .Returns(true);
            Mock.Get(mockKeyboardController)
                .Setup(x => x.Nemeio)
                .Returns(mockNemeio);

            await tool.CheckFirmwareUpdateAsync(mockKeyboardController);

            onFirmwareUpdateCheckedCalled.Should().BeTrue();
            updateFound.Should().BeTrue();
            proxy.Should().NotBeNull();
        }

        [Test]
        public void PackageUpdaterTool_InstallAsync_WhenParametersAreInvalid_Throws()
        {
            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);

            Assert.ThrowsAsync<ArgumentNullException>(() => tool.InstallAsync(null));
        }

        [Test]
        public async Task PackageUpdaterTool_InstallAsync_WhenErrorOccured_RaiseEvent()
        {
            var onInstallFinishedCalled = false;
            Exception installException = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnEmbeddedInstallFinished += (object sender, InstallFinishedEventArgs eventArgs) =>
            {
                onInstallFinishedCalled = true;
                installException = eventArgs.Exception;
            };

            var mockUpdatable = Mock.Of<IUpdatable>();
            Mock.Get(mockUpdatable)
                .Setup(x => x.UpdateAsync())
                .Throws(new InvalidOperationException("This is a test error"));

            await tool.InstallAsync(mockUpdatable);

            onInstallFinishedCalled.Should().BeTrue();
            installException.Should().NotBeNull();
        }

        [Test]
        public async Task PackageUpdaterTool_InstallAsync_WhenUpdatableReturnError_RaiseEvent()
        {
            var eventSemaphore = new SemaphoreSlim(0, 1);

            var onInstallFinishedCalled = false;
            Exception installException = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnEmbeddedInstallFinished += (object sender, InstallFinishedEventArgs eventArgs) =>
            {
                onInstallFinishedCalled = true;
                installException = eventArgs.Exception;

                eventSemaphore.Release();
            };

            var mockUpdatable = Mock.Of<IUpdatable>();
            
            await tool.InstallAsync(mockUpdatable);

            //  Don't want to await this
            var _ = Task.Run(async () => 
            {
                await Task.Delay(30);

                Mock.Get(mockUpdatable)
                    .Raise((updatable) => updatable.OnUpdateFinished += null, mockUpdatable, new UpdateFinishedEventArgs(Errors.ErrorCode.AclKeyboardResponseFirmwareUpdateFailed));
            });
            
            var succeed = await eventSemaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!succeed)
            {
                throw new InvalidOperationException("Event OnInstallFinished never called");
            }

            onInstallFinishedCalled.Should().BeTrue();
            installException.Should().NotBeNull();
        }

        [Test]
        public async Task PackageUpdaterTool_InstallAsync_WhenUpdatableReturnSuccess_RaiseEvent()
        {
            var eventSemaphore = new SemaphoreSlim(0, 1);

            var onInstallFinishedCalled = false;
            Exception installException = null;

            var loggerFactory = new LoggerFactory();

            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();
            var mockInformationService = Mock.Of<IInformationService>();
            var tool = new PackageUpdaterTool(loggerFactory, mockApplicationSettingsManager, mockInformationService);
            tool.OnEmbeddedInstallFinished += (object sender, InstallFinishedEventArgs eventArgs) =>
            {
                onInstallFinishedCalled = true;
                installException = eventArgs.Exception;

                eventSemaphore.Release();
            };

            var mockUpdatable = Mock.Of<IUpdatable>();

            await tool.InstallAsync(mockUpdatable);

            //  Don't want to await this
            var _ = Task.Run(async () =>
            {
                await Task.Delay(30);

                Mock.Get(mockUpdatable)
                    .Raise((updatable) => updatable.OnUpdateFinished += null, mockUpdatable, new UpdateFinishedEventArgs(Errors.ErrorCode.Success));
            });

            var succeed = await eventSemaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!succeed)
            {
                throw new InvalidOperationException("Event OnInstallFinished never called");
            }

            onInstallFinishedCalled.Should().BeTrue();
            installException.Should().BeNull();
        }
    }
}
