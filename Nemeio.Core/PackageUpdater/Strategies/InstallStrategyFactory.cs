using System;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard;
using Nemeio.Core.PackageUpdater.Informations;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater.Strategies
{
    public class InstallStrategyFactory : IInstallStrategyFactory
    {
        private readonly IPackageUpdaterFileProvider _fileProvider;
        private readonly IApplicationService _applicationService;
        private readonly IKeyboardController _keyboardController;
        private readonly IApplicationManifest _applicationManifest;
        private readonly IDocument _document;
        private readonly IFileSystem _fileSystem;

        public InstallStrategyFactory(IPackageUpdaterFileProvider fileProvider, IApplicationService applicationService, IKeyboardController keyboardController, IApplicationManifest applicationManifest, IDocument document, IFileSystem fileSystem)
        {
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _applicationManifest = applicationManifest ?? throw new ArgumentNullException(nameof(applicationManifest));
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public IInstallStrategy CreateApplicationStrategy(DownloadablePackageInformation packageInfo)
        {
            var strategy = new InstallApplicationStrategy(packageInfo, _fileProvider, _applicationService);

            return strategy;
        }

        public IInstallStrategy CreateFirmwareStrategy(FirmwareUpdatableNemeioProxy proxy)
        {
            var strategy = new InstallFirmwareStrategy(_keyboardController, _applicationManifest, _document, _fileProvider, _fileSystem, proxy);

            return strategy;
        }
    }
}
