using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Applications;
using Nemeio.Core.Connectivity;
using Nemeio.Core.DataModels;
using Nemeio.Core.Errors;
using Nemeio.Core.Exceptions;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Nemeios.Proxy;
using Nemeio.Core.PackageUpdater.Downloader;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.PackageUpdater.Updatable;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater.Tools
{
    public class PackageUpdaterTool : IPackageUpdaterTool
    {
        private readonly IApplicationSettingsProvider _applicationSettingsManager;
        private readonly IInformationService _informationService;
        private readonly ILogger _logger;

        private IUpdatable _updatable;

        public event EventHandler<DownloadFinishedEventArgs> OnDownloadFinished;
        public event EventHandler<InternetIsAvailableEventArgs> OnInternetStateComputed;
        public event EventHandler<ApplicationUpdateCheckedEventArgs> OnApplicationUpdateChecked;
        public event EventHandler<FirmwareUpdateCheckedEventArgs> OnFirmwareUpdateChecked;
        public event EventHandler<InstallFinishedEventArgs> OnEmbeddedInstallFinished;
        public event EventHandler<SoftwareInstallFinishedEventArgs> OnSoftwareInstallFinished;

        public PackageUpdaterTool(ILoggerFactory loggerFactory, IApplicationSettingsProvider applicationSettingsManager, IInformationService informationService)
        {
            _logger = loggerFactory.CreateLogger<PackageUpdaterTool>();
            _applicationSettingsManager = applicationSettingsManager ?? throw new ArgumentNullException(nameof(applicationSettingsManager));
            _informationService = informationService ?? throw new ArgumentNullException(nameof(informationService));
        }

        public Task CheckSoftwareUpdateAsync()
        {
            var checkSoftwareUpdateTask = Task.Run(() => 
            {
                var updateTo = _applicationSettingsManager.ApplicationSettings.UpdateTo;
                Exception updateException = null;

                if (updateTo != null)
                {
                    _logger.LogInformation($"Application tried to update to version <{updateTo}>");

                    var applicationVersion = _informationService.GetApplicationVersion();
                    var updateVersion = new VersionProxy(updateTo);

                    if (!applicationVersion.IsEqualTo(updateVersion))
                    {
                        _logger.LogInformation($"Application update failed, application still on version <{applicationVersion}> but target <{updateTo}>");

                        updateException = new InvalidOperationException("Update failed");
                    }
                    else
                    {
                        _logger.LogInformation($"Application update succeed");
                    }
                }

                var eventArgs = new SoftwareInstallFinishedEventArgs(updateTo != null, updateException);

                OnSoftwareInstallFinished?.Invoke(this, eventArgs);
            });

            return checkSoftwareUpdateTask;
        }

        public async Task DownloadDependenciesAsync(IUpdatable updatable, IPackagesDownloader downloader)
        {
            if (updatable == null)
            {
                throw new ArgumentNullException(nameof(updatable));
            }

            if (downloader == null)
            {
                throw new ArgumentNullException(nameof(downloader));
            }

            _logger.LogInformation($"Start download dependencies for <{updatable.GetType().Name}>");

            try
            {
                await updatable.DownloadDependenciesAsync(downloader);

                _logger.LogInformation($"Download dependencies succeed for <{updatable.GetType().Name}>");

                OnDownloadFinished?.Invoke(this, new DownloadFinishedEventArgs(true));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Download dependencies failed for <{updatable.GetType().Name}>");

                OnDownloadFinished?.Invoke(this, new DownloadFinishedEventArgs(false));
            }
        }

        public async Task CheckInternetConnectionAsync(INetworkConnectivityChecker checker)
        {
            if (checker == null)
            {
                throw new ArgumentNullException(nameof(checker));
            }

            try
            {
                var hasInternet = await checker.InternetIsAvailable();

                _logger.LogInformation( $"Check internet connection hasInternet : <{hasInternet}>");

                OnInternetStateComputed?.Invoke(this, new InternetIsAvailableEventArgs(hasInternet));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Check internet connection failed");

                OnInternetStateComputed?.Invoke(this, new InternetIsAvailableEventArgs(false));
            }
        }

        public async Task CheckApplicationUpdateAsync(IPackageUpdateChecker updateChecker)
        {
            if (updateChecker == null)
            {
                throw new ArgumentNullException(nameof(updateChecker));
            }

            try
            {
                var appUpdate = await updateChecker.ApplicationNeedUpdateAsync();

                var founded = appUpdate != null;
                var packages = new List<DownloadablePackageInformation>();

                if (founded)
                {
                    packages.Add(appUpdate);
                }

                OnApplicationUpdateChecked?.Invoke(this, new ApplicationUpdateCheckedEventArgs(founded, packages));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"CheckApplicationUpdateAsync failed");

                OnApplicationUpdateChecked?.Invoke(this, new ApplicationUpdateCheckedEventArgs(false, new List<DownloadablePackageInformation>()));
            }
        }

        public Task CheckFirmwareUpdateAsync(IKeyboardController keyboardController)
        {
            if (keyboardController == null)
            {
                throw new ArgumentNullException(nameof(keyboardController));
            }

            var checkFirmwareUpdate = Task.Run(() =>
            {
                try
                {
                    bool founded = false;
                    FirmwareUpdatableNemeioProxy proxy = null;

                    if (keyboardController.Connected)
                    {
                        proxy = KeyboardProxy.CastTo<FirmwareUpdatableNemeioProxy>(keyboardController.Nemeio);

                        founded = proxy != null;

                        _logger.LogInformation($"Firmware update found");
                    }
                    else
                    {
                        _logger.LogInformation($"Firmware update not found");
                    }

                    OnFirmwareUpdateChecked?.Invoke(this, new FirmwareUpdateCheckedEventArgs(founded, proxy));
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"CheckFirmwareUpdateAsync failed");

                    OnFirmwareUpdateChecked?.Invoke(this, new FirmwareUpdateCheckedEventArgs(false, null));
                }
            });

            return checkFirmwareUpdate;
        }

        public async Task InstallAsync(IUpdatable updatable)
        {
            if (updatable == null)
            {
                throw new ArgumentNullException(nameof(updatable));
            }

            _updatable = updatable;

            try
            {
                _updatable.OnUpdateFinished += Component_OnUpdateFinished;

                _logger.LogInformation($"Start install update on <{updatable.GetType().Name}>");

                await updatable.UpdateAsync();
            }
            //  We want to catch every exception
            catch (Exception exception)
            {
                _updatable = null;
                _logger.LogError(exception, $"InstallAsync failed");

                OnEmbeddedInstallFinished?.Invoke(this, new InstallFinishedEventArgs(exception));
            }
        }

        private void Component_OnUpdateFinished(object sender, Updatable.States.UpdateFinishedEventArgs e)
        {
            if (_updatable != null)
            {
                _updatable.OnUpdateFinished -= Component_OnUpdateFinished;
                _updatable = null;
            }

            _logger.LogInformation($"Update finished with error code <{e.ErrorCode}>");

            if (e.ErrorCode == ErrorCode.Success)
            {
                OnEmbeddedInstallFinished?.Invoke(this, new InstallFinishedEventArgs(null));
            }
            else
            {
                OnEmbeddedInstallFinished?.Invoke(this, new InstallFinishedEventArgs(new CoreException(e.ErrorCode)));
            }
        }
    }
}
