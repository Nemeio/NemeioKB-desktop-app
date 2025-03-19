using System;
using Nemeio.Core.Icon;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Theme;
using Nemeio.Presentation.Menu.Icon;

namespace Nemeio.Wpf.Icon
{
    public class WinApplicationIconProvider : ApplicationIconProvider
    {
        private ThemeIconSet lightIconSet = new ThemeIconSet()
        {
            DisconnectIconPath = "/Icons/pas_connecte_Claire.ico",
            BluetoothLESyncingIconPath = "/Icons/Notification_version_claire.ico",
            BluetoothLEConnectedIconPath = "/Icons/BLE_version_claire.ico",
            UsbSyncingIconPath = "/Icons/Notification_version_claire.ico",
            UsbConnectedIconPath = "/Icons/USB_version_claire.ico",
            UpdateAvailableIconPath = "/Icons/Notification_version_claire.ico",
            UpdateNeededIconPath = "/Icons/Notification_version_claire.ico",
            UpdateAvailableNotConnectedIconPath = "/Icons/pas-connecte-fond-blanc2.ico"
        };
        private ThemeIconSet darkIconSet = new ThemeIconSet()
        {
            DisconnectIconPath = "/Icons/pas_connecte_sombre.ico",
            BluetoothLESyncingIconPath = "/Icons/Notification_version_sombre.ico",
            BluetoothLEConnectedIconPath = "/Icons/BLE_version_sombre.ico",
            UsbSyncingIconPath = "/Icons/Notification_version_sombre.ico",
            UsbConnectedIconPath = "/Icons/USB_version_sombre.ico",
            UpdateAvailableIconPath = "/Icons/Notification_version_sombre.ico",
            UpdateNeededIconPath = "/Icons/Notification_version_sombre.ico",
            UpdateAvailableNotConnectedIconPath = "/Icons/pas-connecte-fond-noir-2.ico"
        };

        public WinApplicationIconProvider(ISynchronizer synchronizer, IKeyboardController keyboardController, IPackageUpdater packageUpdater, ISystemThemeProvider systemThemeProvider)
            : base(keyboardController, synchronizer, packageUpdater, systemThemeProvider) { }

        public override string GetIconResourceFromCurrentState()
        {
            var isDark = _systemThemeProvider.GetSystemTheme() == SystemTheme.Dark;
            var iconType = GetIconFromCurrentState();
            var activeIconSet = isDark ? darkIconSet : lightIconSet;
            switch (iconType)
            {
                case ApplicationIconType.Disconnected:
                    return activeIconSet.DisconnectIconPath;
                case ApplicationIconType.BluetoothLESyncing:
                    return activeIconSet.BluetoothLESyncingIconPath;
                case ApplicationIconType.BluetoothLEConnected:
                    return activeIconSet.BluetoothLEConnectedIconPath;
                case ApplicationIconType.UsbSyncing:
                    return activeIconSet.UsbSyncingIconPath;
                case ApplicationIconType.UsbConnected:
                    return activeIconSet.UsbConnectedIconPath;
                case ApplicationIconType.UpdateAvailable:
                    return activeIconSet.UpdateAvailableIconPath;
                case ApplicationIconType.UpdateAvailableNotConnected:
                    return activeIconSet.UpdateAvailableNotConnectedIconPath;
                case ApplicationIconType.UpdateNeeded:
                    return activeIconSet.UpdateNeededIconPath;
                default:
                    throw new InvalidOperationException("Unknown icon type");
            }
        }
    }
}
