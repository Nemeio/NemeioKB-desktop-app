
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Applications;
using Nemeio.Core.Connectivity;
using Nemeio.Core.Errors;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.PackageUpdater.Downloader;
using Nemeio.Core.PackageUpdater.Exceptions;
using Nemeio.Core.PackageUpdater.Tools;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Core.PackageUpdater.Updatable.Factories;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater
{
    public enum PackageUpdateState
    {
        Idle,

        UpdateChecking,
        CheckApplicationInstallation,
        CheckInternetConnection,
        CheckApplicationUpdate,
        CheckFirmwareUpdate,

        Download,
        DownloadPending,
        Downloading,

        Updating,
        UpdatePending,
        ApplyUpdate,
        WaitUsbKeyboard,

        UpdateFailed,
        UpdateSucceed
    }

    public partial class PackageUpdater : IPackageUpdater
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IErrorManager _errorManager;
        private readonly IPackageUpdateChecker _updateChecker;
        private readonly IPackageUpdaterFileProvider _fileProvider;
        private readonly IKeyboardController _keyboardController;
        private readonly INetworkConnectivityChecker _networkConnectivityChecker;
        private readonly IFileSystem _fileSystem;
        private readonly IDocument _document;
        private readonly IUpdatableFactory _updatableFactory;
        private readonly IPackageUpdaterTool _tool;
        private readonly IApplicationSettingsProvider _applicationSettingsManager;

        private PackagesDownloader _downloader;
        private IUpdatable _updatable;

        public bool PendingUpdates => Component != null;
        public IUpdatable Component
        {
            get => _updatable;
            private set
            {
                _updatable = value;

                if (_updatable != null)
                {
                    OnUpdateAvailable?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler OnUpdateAvailable;

        public PackageUpdater(
            ILoggerFactory loggerFactory,
            IDocument document,
            IFileSystem fileSystem,
            IErrorManager errorManager,
            IPackageUpdateChecker updateChecker,
            IPackageUpdaterFileProvider fileProvider,
            IKeyboardController keyboardController,
            INetworkConnectivityChecker networkConnectivityChecker,
            IUpdatableFactory updatableFactory,
            IPackageUpdaterTool tool,
            IApplicationSettingsProvider applicationSettingsManager)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<PackageUpdater>();

            _document = document ?? throw new ArgumentNullException(nameof(document));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _errorManager = errorManager ?? throw new ArgumentNullException(nameof(errorManager));
            _updateChecker = updateChecker ?? throw new ArgumentNullException(nameof(updateChecker));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));

            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _keyboardController.OnKeyboardConnected += KeyboardController_OnKeyboardConnected;
            _keyboardController.OnKeyboardDisconnected += KeyboardController_OnKeyboardDisconnected;

            _networkConnectivityChecker = networkConnectivityChecker ?? throw new ArgumentNullException(nameof(networkConnectivityChecker));
            _updatableFactory = updatableFactory ?? throw new ArgumentNullException(nameof(updatableFactory));

            _downloader = new PackagesDownloader(_loggerFactory, _errorManager, _fileProvider, _fileSystem, _document);

            _tool = tool ?? throw new ArgumentNullException(nameof(tool));
            _tool.OnDownloadFinished += Tool_OnDownloadFinished;
            _tool.OnInternetStateComputed += Tool_OnInternetStateComputed;
            _tool.OnApplicationUpdateChecked += Tool_OnApplicationUpdateChecked;
            _tool.OnFirmwareUpdateChecked += Tool_OnFirmwareUpdateChecked;
            _tool.OnEmbeddedInstallFinished += Tool_OnInstallFinished;
            _tool.OnSoftwareInstallFinished += Tool_OnSoftwareInstallFinished;

            _applicationSettingsManager = applicationSettingsManager ?? throw new ArgumentNullException(nameof(applicationSettingsManager));

            ConfigureStateMachine();
        }

        #region Events

        private void Tool_OnDownloadFinished(object sender, DownloadFinishedEventArgs e)
        {
            if (e.IsSuccess)
            {
                _logger.LogInformation("Dependencies download succeed");
            }
            else
            {
                _logger.LogInformation("Dependencies download failed");
            }

            DownloadFinished(e.IsSuccess);
        }

        private void Tool_OnInternetStateComputed(object sender, InternetIsAvailableEventArgs e)
        {
            if (e.IsAvailable)
            {
                _logger.LogInformation("Internet connection is available");
            }
            else
            {
                _logger.LogInformation("Not internet connection available");
            }

            InternetStateChanged(e.IsAvailable);
        }

        private void Tool_OnApplicationUpdateChecked(object sender, ApplicationUpdateCheckedEventArgs e)
        {
            if (e.Found)
            {
                var package = e.Packages.First();
                var updatable = _updatableFactory.CreateUpdatableSoftware(package);
                updatable.AddDownloadableDependency(package);

                _logger.LogInformation("Application update is available");

                ApplicationUpdateIsAvailable(updatable);
            }
            else
            {
                _logger.LogInformation("Application update is not available");

                ApplicationUpdateIsAvailable(null);
            }
        }

        private void Tool_OnFirmwareUpdateChecked(object sender, FirmwareUpdateCheckedEventArgs e)
        {
            IUpdatable updatable = null;

            if (e.Found)
            {
                updatable = _updatableFactory.CreateUpdatableKeyboard(e.Proxy);

                _logger.LogInformation("Firmware update is available");
            }
            else
            {
                _logger.LogInformation("Firmware update not available");
            }

            FirmwareUpdateChecked(updatable);
        }

        private void Tool_OnInstallFinished(object sender, InstallFinishedEventArgs e)
        {
            var failed = e.Exception != null;
            if (failed)
            {
                if (e.Exception is InstallFailedException installFailedException && installFailedException.Reason == InstallFailReason.NotUsbCommunication)
                {
                    _logger.LogWarning(e.Exception, "Keyboard is not plugged with USB");

                    NoUsbConnected();
                }
                else
                {
                    _logger.LogError(e.Exception, "Apply update failed");

                    UpdateFinished(e.Exception);
                }
            }
            else
            {
                _logger.LogInformation(e.Exception, "Apply update succeed");

                UpdateFinished(e.Exception);
            }
        }

        private void Tool_OnSoftwareInstallFinished(object sender, SoftwareInstallFinishedEventArgs e)
        {
            if (!e.HasTryUpdate)
            {
                _logger.LogInformation("No software update done");

                NoApplicationInstallationFound();
            }
            else
            {
                var failed = e.Exception != null;
                if (failed)
                {
                    _logger.LogError(e.Exception, "Software update failed");
                }
                else
                {
                    _logger.LogInformation("Software update succeed");
                }

                ApplicationInstallationFinished(!failed);
            }
        }

        #endregion

        /// <summary>
        /// Check if an application update is available. 
        /// Check if an update is available. 
        /// First we always check application update. 
        /// Then if there is no application's update we check keyboard's update.
        /// </summary>
        /// <returns></returns>
        public async Task CheckUpdatesAsync() => await CheckInstallAsync();

        /// <summary>
        /// An update is pending, we start download it.
        /// </summary>
        /// <returns></returns>
        public async Task DownloadPendingUpdateAsync() => await InternalDownloadAsync();

        /// <summary>
        /// Install pending update.
        /// </summary>
        /// <returns></returns>
        public async Task InstallUpdateAsync() => await InternalUpdateAsync();

        #region Keyboard event

        private async void KeyboardController_OnKeyboardConnected(object sender, EventArgs e)
        {
            var proxy = KeyboardProxy.CastTo<FirmwareUpdatableNemeioProxy>(_keyboardController.Nemeio);

            if (!PendingUpdates && proxy != null)
            {
                await CheckUpdatesAsync();
            }
        }

        private void KeyboardController_OnKeyboardDisconnected(object sender, EventArgs e)
        {
            KeyboardDisconnected();
        }

        #endregion
    }
}