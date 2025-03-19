using System;
using System.Collections.Generic;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Keyboard.Updates.Progress;
using Nemeio.Core.Managers;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Presentation.PackageUpdater.UI;

namespace Nemeio.Presentation.PackageUpdater
{
    public class PackageUpdaterMessageFactory : IPackageUpdaterMessageFactory
    {
        private readonly IPackageUpdater _packageUpdater;
        private readonly ILanguageManager _languageManager;

        private IDictionary<PackageUpdateState, PackageUpdaterMessage> _applicationUpdateMessages;
        private IDictionary<PackageUpdateState, PackageUpdaterMessage> _embeddedUpdateMessages;
        private IDictionary<PackageUpdateState, PackageUpdaterMessage> _neutralUpdateMessages;

        public PackageUpdaterMessageFactory(IPackageUpdater packageUpdater, ILanguageManager languageManager)
        {
            _packageUpdater = packageUpdater ?? throw new ArgumentNullException(nameof(packageUpdater));
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _languageManager.LanguageChanged += LanguageManager_LanguageChanged;

            CreateMessages();
        }

        private void LanguageManager_LanguageChanged(object sender, EventArgs e) => CreateMessages();

        public PackageUpdaterMessage GetMessage(PackageUpdateState step)
        {
            IDictionary<PackageUpdateState, PackageUpdaterMessage> messages;

            //  FIXME: [KSB] Maybe there are a better way to make it
            if (_packageUpdater.Component is UpdatableSoftware)
            {
                messages = _applicationUpdateMessages;
            }
            else if (_packageUpdater.Component is UpdatableKeyboard)
            {
                messages = _embeddedUpdateMessages;
            }
            else
            {
                messages = _neutralUpdateMessages;
            }

            if (messages.ContainsKey(step))
            {
                return messages[step];
            }

            //  By default we return an empty message
            return new PackageUpdaterMessage(string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public StringId GetNameForComponent(UpdateComponent component)
        {
            switch (component)
            {
                case UpdateComponent.System:
                    return StringId.ComponentSystem;
                case UpdateComponent.FlashMemory:
                    return StringId.ComponentFlashMemory;
                case UpdateComponent.FileSystem:
                    return StringId.ComponentFileSystem;
                case UpdateComponent.Display:
                    return StringId.ComponentDisplay;
                case UpdateComponent.BluetoothLE:
                    return StringId.ComponentBluetoothLE;
                case UpdateComponent.FuelGauge:
                    return StringId.ComponentFuelGauge;
                case UpdateComponent.Unknown:
                default:
                    return StringId.ComponentUnknown;
            }
        }

        public StringId GetStatusMessageForCurrentState()
        {
            switch (_packageUpdater.State)
            {
                case PackageUpdateState.Idle:
                case PackageUpdateState.UpdateSucceed:
                case PackageUpdateState.CheckApplicationInstallation:
                    return StringId.PackageUpdaterUpdatedStateTitle;
                case PackageUpdateState.UpdateChecking:
                case PackageUpdateState.CheckInternetConnection:
                case PackageUpdateState.CheckApplicationUpdate:
                case PackageUpdateState.CheckFirmwareUpdate:
                    return StringId.PackageUpdaterCheckingStateTitle;
                case PackageUpdateState.DownloadPending:
                case PackageUpdateState.UpdatePending:
                case PackageUpdateState.Downloading:
                case PackageUpdateState.WaitUsbKeyboard:
                    if (_packageUpdater.Component is UpdatableKeyboard)
                    {
                        return StringId.PackageUpdaterNeedToBeUpdatedStateTitle;
                    }
                    else
                    {
                        return StringId.PackageUpdaterCanBeUpdatedStateTitle;
                    }
                case PackageUpdateState.ApplyUpdate:
                    return StringId.PackageUpdaterInstallingTitle;
                case PackageUpdateState.UpdateFailed:
                    return StringId.PackageUpdaterErrorStateTitle;
                default:
                    throw new InvalidOperationException("Unknown update kind");
            }
        }

        private void CreateMessages()
        {
            _applicationUpdateMessages = new Dictionary<PackageUpdateState, PackageUpdaterMessage>();
            _applicationUpdateMessages.Add(
                PackageUpdateState.DownloadPending,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterUpdateAvailableTitle),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterDesktopAppAvailableUpdateMessage),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterUpdateAvailableDownloadButton),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterDownloadEndCloseButton)
                )
            );

            _applicationUpdateMessages.Add(
                PackageUpdateState.UpdatePending,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterDownloadEndTitle),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterDownloadEndSubtitle),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterDownloadEndInstallButton),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterDownloadEndCloseButton)
                )
            );

            _applicationUpdateMessages.Add(
                PackageUpdateState.Downloading,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterDownloadTitle),
                    string.Empty,
                    string.Empty,
                    _languageManager.GetLocalizedValue(StringId.CommonOk)
                )
            );

            _applicationUpdateMessages.Add(
                PackageUpdateState.UpdateFailed,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterErrorTitle),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterErrorApplicationSubtitle),
                    string.Empty,
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterErrorCloseButton)
                )
            );

            _embeddedUpdateMessages = new Dictionary<PackageUpdateState, PackageUpdaterMessage>();
            _embeddedUpdateMessages.Add(
                PackageUpdateState.UpdatePending,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterNeedToBeUpdatedStateTitle),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterKeyboardUsbConnectedUpdateMessage),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterDownloadEndInstallButton),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterDownloadEndCloseButton)
                )
            );
            _embeddedUpdateMessages.Add(
                PackageUpdateState.ApplyUpdate,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterInstallingTitle),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterInstallingPopUpMessage),
                    string.Empty,
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterInstallingCloseButton)
                )
            );
            _embeddedUpdateMessages.Add(
                PackageUpdateState.UpdateFailed,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterErrorTitle),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterErrorEmbeddedSubtitle),
                    string.Empty,
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterErrorCloseButton)
                )
            );
            _embeddedUpdateMessages.Add(
                PackageUpdateState.WaitUsbKeyboard,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterNoUsbConnectionTitle),
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterNoUsbConnectionSubtitle),
                    string.Empty,
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterNoUsbConnectionRetryButton)
                )
            );
            _embeddedUpdateMessages.Add(
                PackageUpdateState.UpdateSucceed,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterEmbeddedInstallSucceed),
                    string.Empty,
                    string.Empty,
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterErrorCloseButton)
                )
            );

            _neutralUpdateMessages = new Dictionary<PackageUpdateState, PackageUpdaterMessage>();
            _neutralUpdateMessages.Add(
                PackageUpdateState.UpdateSucceed,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterSoftwareInstallSucceed),
                    string.Empty,
                    string.Empty,
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterErrorCloseButton)
                )
            );
            _neutralUpdateMessages.Add(
                PackageUpdateState.UpdateFailed,
                new PackageUpdaterMessage(
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterSoftwareInstallFailed),
                    string.Empty,
                    string.Empty,
                    _languageManager.GetLocalizedValue(StringId.PackageUpdaterErrorCloseButton)
                )
            );
        }
    }
}
