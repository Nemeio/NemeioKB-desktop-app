using FluentAssertions;
using Moq;
using Nemeio.Core.Icon;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Theme;
using Nemeio.Wpf.Icon;
using NUnit.Framework;

namespace Nemeio.Wpf.Test.Icon
{
    //  FIXME
    /*public class WinApplicationIconProviderShould
    {
        private WinApplicationIconProvider _applicationIconProvider;

        [SetUp]
        public void SetUp()
        {
            var mockPackageUpdater = Mock.Of<IPackageUpdater>();
            var mockPackageUpdaterFollower = Mock.Of<IPackageUpdaterFollower>();
            var mockSystemThemeProvider = Mock.Of<ISystemThemeProvider>();
            var mockKeyboardController = Mock.Of<IKeyboardController>();
            var mockSynchronizer = Mock.Of<ISynchronizer>();

            _applicationIconProvider = new WinApplicationIconProvider(mockSynchronizer, mockKeyboardController, mockPackageUpdater, mockPackageUpdaterFollower, mockSystemThemeProvider);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenNoKeyboardPlugged_ReturnDisconnectedType()
        {
            var iconType = _applicationIconProvider.GetIconFromCurrentState();

            iconType.Should().Be(ApplicationIconType.Disconnected);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenKeyboardPluggedWithUsb_AndSyncing_ReturnUsbSyncingType()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Usb;

            var mockConnectionManager = CreateMockConnectionManager(communicationType);
            var deviceSynchronizer = CreateLayoutSynchronizer(communicationType);

            var iconType = _applicationIconProvider.GetIconFromCurrentState(mockConnectionManager, PortStatus.Busy, deviceSynchronizer);
            iconType.Should().Be(ApplicationIconType.UsbSyncing);

            iconType = _applicationIconProvider.GetIconFromCurrentState(mockConnectionManager, PortStatus.Busy, null);
            iconType.Should().Be(ApplicationIconType.UsbSyncing);

            iconType = _applicationIconProvider.GetIconFromCurrentState(mockConnectionManager, PortStatus.Ready, deviceSynchronizer);
            iconType.Should().Be(ApplicationIconType.UsbSyncing);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenKeyboardPluggedWithUsb_AndNotSyncing_ReturnUsbConnectedType()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Usb;

            var mockConnectionManager = CreateMockConnectionManager(communicationType);

            var iconType = _applicationIconProvider.GetIconFromCurrentState(mockConnectionManager, PortStatus.Ready, null);

            iconType.Should().Be(ApplicationIconType.UsbConnected);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenKeyboardPluggedWithBle_AndSyncing_ReturnBleSyncingType()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Ble;

            var mockConnectionManager = CreateMockConnectionManager(communicationType);
            var deviceSynchronizer = CreateLayoutSynchronizer(communicationType);

            var iconType = _applicationIconProvider.GetIconFromCurrentState(mockConnectionManager, PortStatus.Busy, deviceSynchronizer);
            iconType.Should().Be(ApplicationIconType.BluetoothLESyncing);

            iconType = _applicationIconProvider.GetIconFromCurrentState(mockConnectionManager, PortStatus.Busy, null);
            iconType.Should().Be(ApplicationIconType.BluetoothLESyncing);

            iconType = _applicationIconProvider.GetIconFromCurrentState(mockConnectionManager, PortStatus.Ready, deviceSynchronizer);
            iconType.Should().Be(ApplicationIconType.BluetoothLESyncing);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenKeyboardPluggedWithBle_AndNotSyncing_ReturnBleConnectedType()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Ble;

            var mockConnectionManager = CreateMockConnectionManager(communicationType);

            var iconType = _applicationIconProvider.GetIconFromCurrentState(mockConnectionManager, PortStatus.Ready, null);

            iconType.Should().Be(ApplicationIconType.BluetoothLEConnected);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenUpdateIsAvailable_ReturnUpdateAvailableType()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Usb;
            const PackageType packageType = PackageType.Application;
            const PackageUpdateKind updateKind = PackageUpdateKind.CanUpdate;

            var iconType = TestIconWithUpdateParameters(communicationType, packageType, updateKind, null);

            iconType.Should().Be(ApplicationIconType.UpdateAvailable);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenUpdateIsNeeded_ReturnUpdateNeededType()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Usb;
            const PackageType packageType = PackageType.Application;
            const PackageUpdateKind updateKind = PackageUpdateKind.NeedUpdate;

            var iconType = TestIconWithUpdateParameters(communicationType, packageType, updateKind, null);

            iconType.Should().Be(ApplicationIconType.UpdateNeeded);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenUpdateIsAvailable_AndSyncing_OnUsb_ReturnUsbSyncing()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Usb;
            const PackageType packageType = PackageType.Application;
            const PackageUpdateKind updateKind = PackageUpdateKind.CanUpdate;

            var mockDeviceSynchronizer = CreateLayoutSynchronizer(communicationType);
            var iconType = TestIconWithUpdateParameters(communicationType, packageType, updateKind, mockDeviceSynchronizer);

            iconType.Should().Be(ApplicationIconType.UsbSyncing);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenUpdateIsNeeded_AndSyncing_OnUsb_ReturnUsbSyncingType()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Usb;
            const PackageType packageType = PackageType.Application;
            const PackageUpdateKind updateKind = PackageUpdateKind.NeedUpdate;

            var mockDeviceSynchronizer = CreateLayoutSynchronizer(communicationType);
            var iconType = TestIconWithUpdateParameters(communicationType, packageType, updateKind, mockDeviceSynchronizer);

            iconType.Should().Be(ApplicationIconType.UsbSyncing);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenUpdateIsAvailable_AndSyncing_OnBle_ReturnBleSyncing()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Ble;
            const PackageType packageType = PackageType.Firmware;
            const PackageUpdateKind updateKind = PackageUpdateKind.CanUpdate;

            var mockDeviceSynchronizer = CreateLayoutSynchronizer(communicationType);
            var iconType = TestIconWithUpdateParameters(communicationType, packageType, updateKind, mockDeviceSynchronizer);

            iconType.Should().Be(ApplicationIconType.BluetoothLESyncing);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconFromCurrentState_WhenUpdateIsNeeded_AndSyncing_OnBle_ReturnUpdateNeededType()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Ble;
            const PackageType packageType = PackageType.Firmware;
            const PackageUpdateKind updateKind = PackageUpdateKind.NeedUpdate;

            var mockDeviceSynchronizer = CreateLayoutSynchronizer(communicationType);
            var iconType = TestIconWithUpdateParameters(communicationType, packageType, updateKind, mockDeviceSynchronizer);

            iconType.Should().Be(ApplicationIconType.BluetoothLESyncing);
        }

        private ApplicationIconType TestIconWithUpdateParameters(DeviceCommunicationType communicationType, PackageType packageType, PackageUpdateKind updateKind, DeviceLayoutSynchronizer synchronizer)
        {
            var portStatus = synchronizer == null ? PortStatus.Ready : PortStatus.Busy;
            var mockPackageUpdater = Mock.Of<IPackageUpdater>();
            Mock.Get(mockPackageUpdater)
                .Setup(x => x.PendingUpdates)
                .Returns(true);

            Mock.Get(mockPackageUpdater)
                .Setup(x => x.PendingUpdateType)
                .Returns(packageType);

            var mockPackageUpdaterFollower = Mock.Of<IPackageUpdaterFollower>();

            Mock.Get(mockPackageUpdaterFollower)
                .Setup(x => x.UpdateKind)
                .Returns(updateKind);

            var mockSystemThemeProvider = Mock.Of<ISystemThemeProvider>();
            var mockConnectionManager = CreateMockConnectionManager(communicationType);

            var applicationIconProvider = new WinApplicationIconProvider(mockPackageUpdater, mockPackageUpdaterFollower, mockSystemThemeProvider);

            var iconType = applicationIconProvider.GetIconFromCurrentState(mockConnectionManager, portStatus, synchronizer);

            return iconType;
        }

        [Test]
        public void WinApplicationIconProvider_GetIconResourceFromCurrentState_WhenNoKeyboardPlugged_ReturnDisconnectedTypeIconPath()
        {
            var mockConnectionManager = Mock.Of<IConnectionManager>();

            var iconResource = _applicationIconProvider.GetIconResourceFromCurrentState(mockConnectionManager, PortStatus.None, null);
            iconResource.Should().Be(WinApplicationIconProvider.DisconnectIconPath);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconResourceFromCurrentState_WhenKeyboardPluggedWithUsb_AndSyncing_ReturnUsbSyncingIconPath()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Usb;

            var mockConnectionManager = CreateMockConnectionManager(communicationType);
            var deviceSynchronizer = CreateLayoutSynchronizer(communicationType);

            var iconResource = _applicationIconProvider.GetIconResourceFromCurrentState(mockConnectionManager, PortStatus.Busy, deviceSynchronizer);
            iconResource.Should().Be(WinApplicationIconProvider.UsbSyncingIconPath);

            iconResource = _applicationIconProvider.GetIconResourceFromCurrentState(mockConnectionManager, PortStatus.Busy, null);
            iconResource.Should().Be(WinApplicationIconProvider.UsbSyncingIconPath);

            iconResource = _applicationIconProvider.GetIconResourceFromCurrentState(mockConnectionManager, PortStatus.Ready, deviceSynchronizer);
            iconResource.Should().Be(WinApplicationIconProvider.UsbSyncingIconPath);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconResourceFromCurrentState_WhenKeyboardPluggedWithUsb_AndNotSyncing_ReturnUsbConnectedIconPath()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Usb;

            var mockConnectionManager = CreateMockConnectionManager(communicationType);

            var iconResource = _applicationIconProvider.GetIconResourceFromCurrentState(mockConnectionManager, PortStatus.Ready, null);
            iconResource.Should().Be(WinApplicationIconProvider.UsbConnectedIconPath);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconResourceFromCurrentState_WhenKeyboardPluggedWithBle_AndSyncing_ReturnBleSyncingIconPath()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Ble;

            var mockConnectionManager = CreateMockConnectionManager(communicationType);
            var deviceSynchronizer = CreateLayoutSynchronizer(communicationType);

            var iconResource = _applicationIconProvider.GetIconResourceFromCurrentState(mockConnectionManager, PortStatus.Busy, deviceSynchronizer);
            iconResource.Should().Be(WinApplicationIconProvider.BluetoothLESyncingIconPath);

            iconResource = _applicationIconProvider.GetIconResourceFromCurrentState(mockConnectionManager, PortStatus.Busy, null);
            iconResource.Should().Be(WinApplicationIconProvider.BluetoothLESyncingIconPath);

            iconResource = _applicationIconProvider.GetIconResourceFromCurrentState(mockConnectionManager, PortStatus.Ready, deviceSynchronizer);
            iconResource.Should().Be(WinApplicationIconProvider.BluetoothLESyncingIconPath);
        }

        [Test]
        public void WinApplicationIconProvider_GetIconResourceFromCurrentState_WhenKeyboardPluggedWithBle_AndNotSyncing_ReturnBleConnectedIconPath()
        {
            const DeviceCommunicationType communicationType = DeviceCommunicationType.Ble;

            var mockConnectionManager = CreateMockConnectionManager(communicationType);

            var iconResource = _applicationIconProvider.GetIconResourceFromCurrentState(mockConnectionManager, PortStatus.Ready, null);
            iconResource.Should().Be(WinApplicationIconProvider.BluetoothLEConnectedIconPath);
        }
    }*/
}
