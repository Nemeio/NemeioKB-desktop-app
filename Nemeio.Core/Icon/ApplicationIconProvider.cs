using System;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.Layouts.Synchronization;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Core.Theme;

namespace Nemeio.Core.Icon
{
    public abstract class ApplicationIconProvider : IApplicationIconProvider
    {
        protected readonly IPackageUpdater _packageUpdater;
        protected readonly ISystemThemeProvider _systemThemeProvider;
        protected readonly ISynchronizer _synchronizer;
        protected readonly IKeyboardController _keyboardController;

        public ApplicationIconProvider(IKeyboardController keyboardController, ISynchronizer synchronizer, IPackageUpdater packageUpdater, ISystemThemeProvider themeProvider)
        {
            _packageUpdater = packageUpdater ?? throw new ArgumentNullException(nameof(packageUpdater));
            _systemThemeProvider = themeProvider ?? throw new ArgumentNullException(nameof(themeProvider));
            _synchronizer = synchronizer ?? throw new ArgumentNullException(nameof(synchronizer));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
        }

        public ApplicationIconType GetIconFromCurrentState()
        {
            if (!_keyboardController.Connected)
            {
                return GetIconTypeIfNoUpdate(ApplicationIconType.Disconnected);
            }

            var communicationType = _keyboardController.Nemeio.CommunicationType;
            var synchronizing = _synchronizer.State == Synchronizer.SynchornizerState.Syncing;

            switch (communicationType)
            {
                case CommunicationType.Serial:
                    if (synchronizing)
                    {
                        return ApplicationIconType.UsbSyncing;
                    }
                    return GetIconTypeIfNoUpdate(ApplicationIconType.UsbConnected);

                case CommunicationType.BluetoothLE:
                    if (synchronizing)
                    {
                        return ApplicationIconType.BluetoothLESyncing;
                    }
                    return GetIconTypeIfNoUpdate(ApplicationIconType.BluetoothLEConnected);

                default:
                    throw new InvalidOperationException("Unknown communication type");
            }
        }

        public abstract string GetIconResourceFromCurrentState();

        private ApplicationIconType GetIconTypeIfNoUpdate(ApplicationIconType iconType)
        {
            if (!_packageUpdater.PendingUpdates)
            {
                return iconType;
            }

            if (_packageUpdater.Component is UpdatableKeyboard)
            {
                return ApplicationIconType.UpdateNeeded;
            }
            else if (_packageUpdater.Component is UpdatableSoftware)
            {
                if (_keyboardController.Connected)
                    return ApplicationIconType.UpdateAvailable;
                else
                    return ApplicationIconType.UpdateAvailableNotConnected;
            }
            else
            {
                return iconType;
            }
        }
    }
}
