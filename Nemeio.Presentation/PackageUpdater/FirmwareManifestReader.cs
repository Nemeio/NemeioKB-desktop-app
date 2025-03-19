using System;
using System.IO;
using System.Threading.Tasks;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.FileSystem;
using Nemeio.Core.PackageUpdater;
using Nemeio.Presentation.PackageUpdater.Firmware;
using Newtonsoft.Json;

namespace Nemeio.Presentation.PackageUpdater
{
    public class FirmwareManifestReader : IFirmwareManifestReader
    {
        private readonly IFileSystem _fileSystem;

        public FirmwareManifestReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task<FirmwareManifest> ReadAsync(string path)
        {
            try
            {
                var content = await _fileSystem.ReadTextAsync(path);
                var packageManifestDto = JsonConvert.DeserializeObject<FirmwarePackageManifestDto>(content);
                var packageManifest = packageManifestDto.ToDomainModel();
                return packageManifest;
            }
            catch (FileNotFoundException)
            {
                //  Only for debug
                //  Package is compiled and embedded by pipeline but not on debug

                return null;
            }
        }
    }
}
