using System;
using System.Threading.Tasks;
using Nemeio.Tools.Firmware.PackageBuilder.Applications.Exceptions;
using Nemeio.Tools.Firmware.PackageBuilder.Builders;
using Nemeio.Tools.Firmware.PackageBuilder.Files;
using Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest;
using Nemeio.Tools.Firmware.PackageBuilder.Firmwares.Composer;
using Nemeio.Tools.Firmware.PackageBuilder.Firmwares.Writer;

namespace Nemeio.Tools.Firmware.PackageBuilder.Applications
{
    internal sealed class Application : IPackageBuilderApplication
    {
        private readonly IInputFileFactory _inputFileFactory;
        private readonly IFirmwareManifestReader _firmwareManifestReader;
        private readonly IPackageFirmwareBuilder _packageFirmwareBuilder;
        private readonly IFirmwareWriter _firmwareWriter;
        private readonly IUpdateInformationComposer _updateComposer;

        public ApplicationStartupSettings Settings { get; set; }

        public Application(IInputFileFactory inputFileFactory, IFirmwareManifestReader firmwareManifestReader, IPackageFirmwareBuilder packageFirmwareBuilder, IFirmwareWriter firmwareWriter, IUpdateInformationComposer infoComposer)
        {
            _inputFileFactory = inputFileFactory ?? throw new ArgumentNullException(nameof(inputFileFactory));
            _firmwareManifestReader = firmwareManifestReader ?? throw new ArgumentNullException(nameof(firmwareManifestReader));
            _packageFirmwareBuilder = packageFirmwareBuilder ?? throw new ArgumentNullException(nameof(packageFirmwareBuilder));
            _firmwareWriter = firmwareWriter ?? throw new ArgumentNullException(nameof(firmwareWriter));
            _updateComposer = infoComposer ?? throw new ArgumentNullException(nameof(infoComposer));
        }

        public async Task RunAsync()
        {
            var manifest = await LoadManifestAsync();

            var updateInformation = ComposeUpdate(manifest);

            await BuildPackageAsync(updateInformation);
        }

        private async Task<FirmwareManifest> LoadManifestAsync()
        {
            try
            {
                var manifestFile = _inputFileFactory.CreateManifestInputFile(Settings.ManifestFilePath);

                var manifest = await _firmwareManifestReader.ReadAsync(manifestFile.FilePath);

                return manifest;
            }
            catch (Exception exception)
            {
                throw new LoadManifestFailedException($"Load manifest failed", exception);
            }
        }

        private FirmwareUpdateInformation ComposeUpdate(FirmwareManifest manifest)
        {
            try
            {
                var information = _updateComposer.Compose(manifest, Settings.InputFileDirectoryPath);

                return information;
            }
            catch (Exception exception)
            {
                throw new ComposeUpdateFailedException($"Compose update failed", exception);
            }
        }

        private async Task BuildPackageAsync(FirmwareUpdateInformation updateInformation)
        {
            try
            {
                var firmware = await _packageFirmwareBuilder.CreatePackageAsync(updateInformation);

                await _firmwareWriter.WriteOnDisk(firmware, Settings.OuputFilePath);
            }
            catch (Exception exception)
            {
                throw new BuildPackageFailedException($"Build package failed", exception);
            }
        }
    }
}
