using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Applications;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.Keyboard;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.PackageUpdater.Strategies;
using Nemeio.Core.Tools;

namespace Nemeio.Core.PackageUpdater.Updatable.Factories
{
    public class UpdatableFactory : IUpdatableFactory
    {
        private readonly IInstallStrategyFactory _installStrategyFactory;
        private readonly IApplicationManifest _applicationManifest;
        private readonly IKeyboardController _keyboardController;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IApplicationSettingsProvider _applicationSettingsManager;

        public UpdatableFactory(ILoggerFactory loggerFactory, IInstallStrategyFactory installStrategyFactory, IApplicationManifest applicationManifest, IKeyboardController keyboardController, IApplicationSettingsProvider applicationSettingsManager)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _installStrategyFactory = installStrategyFactory ?? throw new ArgumentNullException(nameof(installStrategyFactory));
            _applicationManifest = applicationManifest ?? throw new ArgumentNullException(nameof(applicationManifest));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _applicationSettingsManager = applicationSettingsManager ?? throw new ArgumentNullException(nameof(applicationSettingsManager));
        }

        public IUpdatable CreateUpdatableKeyboard(FirmwareUpdatableNemeioProxy proxy)
        {
            var timeoutTimer = new Timer(TimeSpan.FromSeconds(180));
            var strategy = _installStrategyFactory.CreateFirmwareStrategy(proxy);
            var updatable = new UpdatableKeyboard(_loggerFactory, _keyboardController, proxy, strategy, timeoutTimer, _applicationManifest.FirmwareManifest);

            return updatable;
        }

        public IUpdatable CreateUpdatableSoftware(DownloadablePackageInformation packageInfo)
        {
            var strategy = _installStrategyFactory.CreateApplicationStrategy(packageInfo);
            var updatable = new UpdatableSoftware(_loggerFactory, strategy, _applicationSettingsManager, packageInfo.Version);

            return updatable;
        }
    }
}
