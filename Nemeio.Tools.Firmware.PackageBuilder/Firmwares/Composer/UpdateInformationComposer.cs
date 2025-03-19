using System.IO;
using Nemeio.Tools.Firmware.PackageBuilder.Builders;
using Nemeio.Tools.Firmware.PackageBuilder.Files;
using Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest;

namespace Nemeio.Tools.Firmware.PackageBuilder.Firmwares.Composer
{
    internal class UpdateInformationComposer : IUpdateInformationComposer
    {
        private readonly IInputFileFactory _inputFileFactory;

        public UpdateInformationComposer(IInputFileFactory inputFileFactory)
        {
            _inputFileFactory = inputFileFactory ?? throw new System.ArgumentNullException(nameof(inputFileFactory));
        }

        public FirmwareUpdateInformation Compose(FirmwareManifest manifest, string fileDirectoryPath = null)
        {
            var firmwareUpdateInformation = new FirmwareUpdateInformation();
            firmwareUpdateInformation.AddManifest(manifest);

            foreach (var update in firmwareUpdateInformation.Informations)
            {
                var composePath = string.IsNullOrEmpty(fileDirectoryPath)
                    ? Directory.GetCurrentDirectory()
                    : fileDirectoryPath;

                composePath = Path.Combine(composePath, update.Value.Filename);

                var inputFile = _inputFileFactory.CreateFirmwareInputFile(update.Key, composePath);

                update.Value.Path = inputFile.FilePath;
            }

            return firmwareUpdateInformation;
        }
    }
}
