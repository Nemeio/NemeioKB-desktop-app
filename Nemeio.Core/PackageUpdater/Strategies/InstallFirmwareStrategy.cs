using System;
using System.IO;
using System.Threading.Tasks;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Keyboard.Communication;
using Nemeio.Core.PackageUpdater.Exceptions;
using Nemeio.Core.PackageUpdater.Firmware;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater.Strategies
{
    public class InstallFirmwareStrategy : IInstallStrategy
    {
        private const string PackageFileNameWithExtension = "package.bin";

        private readonly IKeyboardController _keyboardController;
        private readonly IApplicationManifest _applicationManifest;
        private readonly IPackageUpdaterFileProvider _fileProvider;
        private readonly IDocument _document;
        private readonly IFileSystem _fileSystem;
        private readonly FirmwareUpdatableNemeioProxy _proxy;

        public InstallFirmwareStrategy(IKeyboardController keyboardController, IApplicationManifest applicationManifest, IDocument document, IPackageUpdaterFileProvider fileProvider, IFileSystem fileSystem, FirmwareUpdatableNemeioProxy proxy)
        {
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _applicationManifest = applicationManifest ?? throw new ArgumentNullException(nameof(applicationManifest));
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _proxy = proxy;
        }

        public async Task InstallAsync()
        {
            if (_proxy != null)
            {
                if (_proxy.CommunicationType != CommunicationType.Serial)
                {
                    throw new InstallFailedException(InstallFailReason.NotUsbCommunication);
                }

                var firmwarePackageFilePath = Path.Combine(
                    _document.FirmwaresFolder,
                    PackageFileNameWithExtension
                );

                var tempFilePath = TransferFirmwareFileToTempPath(firmwarePackageFilePath);

                var fileContent = await _fileSystem.ReadByteArrayAsync(tempFilePath);

                var firmwarePackage = new BinaryFirmware(fileContent);
                var installer = new PackageFirmwareInstaller(firmwarePackage);

                try
                {
                    await installer.InstallAsync(_proxy);
                }
                catch (Exception)
                {
                    throw new InstallFailedException(InstallFailReason.Unknown);
                }
            }
            else
            {
                throw new InstallFailedException(InstallFailReason.KeyboardIsNotConnected);
            }
        }

        private string TransferFirmwareFileToTempPath(string originalFilePath)
        {
            if (string.IsNullOrWhiteSpace(originalFilePath))
            {
                throw new ArgumentNullException(nameof(originalFilePath));
            }

            var tempFolderPath = _document.TemporaryFolderPath;

            //  Create temp folder if not exists
            _fileSystem.CreateDirectoryIfNotExists(tempFolderPath);

            var destinationFilePath = Path.Combine(
                tempFolderPath,
                Path.GetFileName(originalFilePath)
            );

            //  If file already exists we override it
            _fileSystem.RemoveFileIfExists(destinationFilePath);
            _fileSystem.CopyTo(originalFilePath, tempFolderPath);

            return destinationFilePath;
        }
    }
}
