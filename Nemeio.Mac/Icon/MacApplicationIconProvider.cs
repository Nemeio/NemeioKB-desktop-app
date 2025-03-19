using System;
using Nemeio.Core.Icon;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Theme;

namespace Nemeio.Mac.Icon
{
    public class MacApplicationIconProvider : ApplicationIconProvider
    {
        public const string NemeioLoadingImageName = "loading_keyboard";
        public const string NemeioConnectedImageName = "connected_keyboard";
        public const string NemeioDisconnectedImageName = "disconnected_keyboard";
        public const string NemeioBluetoothLoadingImageName = "loading_blue_keyboard";
        public const string NemeioBluetoothConnectedImageName = "connected_blue_keyboard";
        public const string NemeioUpdateAvailableLightImageName = "update_available_light_keyboard";
        public const string NemeioUpdateAvailableDarkImageName = "update_available_dark_keyboard";
        public const string NemeioUpdateNeededLightImageName = "update_needed_light_keyboard";
        public const string NemeioUpdateNeededDarkImageName = "udpdate_needed_dark_keyboard";

        public MacApplicationIconProvider(IKeyboardController keyboardController, ISynchronizer synchronizer, IPackageUpdater packageUpdater, ISystemThemeProvider themeProvider)
            : base(keyboardController, synchronizer, packageUpdater, themeProvider) { }

        public override string GetIconResourceFromCurrentState()
        {
            var isDark = _systemThemeProvider.GetSystemTheme() == SystemTheme.Dark;
            var iconType = GetIconFromCurrentState();
            switch (iconType)
            {
                case ApplicationIconType.Disconnected:
                    return NemeioDisconnectedImageName;
                case ApplicationIconType.BluetoothLESyncing:
                    return NemeioBluetoothLoadingImageName;
                case ApplicationIconType.BluetoothLEConnected:
                    return NemeioBluetoothConnectedImageName;
                case ApplicationIconType.UsbSyncing:
                    return NemeioLoadingImageName;
                case ApplicationIconType.UsbConnected:
                    return NemeioConnectedImageName;
                case ApplicationIconType.UpdateAvailable:
                    if (isDark)
                    {
                        return NemeioUpdateAvailableDarkImageName;
                    }
                    return NemeioUpdateAvailableLightImageName;
                case ApplicationIconType.UpdateNeeded:
                    if (isDark)
                    {
                        return NemeioUpdateNeededDarkImageName;
                    }
                    return NemeioUpdateNeededLightImageName;
                default:
                    throw new InvalidOperationException("Unknown icon type");
            }
        }
    }
}
