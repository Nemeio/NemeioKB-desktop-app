using System;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;
using Newtonsoft.Json;

namespace Nemeio.Tools.Firmware.PackageBuilder
{
    public class NrfManifestReader : INrfManifestReader
    {
        private readonly IFileSystem _fileSystem;

        public NrfManifestReader(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task<NrfManifest> ParseManifest(string manifestFilePath)
        {
            var exists = _fileSystem.FileExists(manifestFilePath);
            if (!exists)
            {
                throw new InvalidOperationException();
            }

            var fileContent = await _fileSystem.ReadTextAsync(manifestFilePath);
            var manifestDto = JsonConvert.DeserializeObject<NrfManifestDto>(fileContent);

            return manifestDto.ToDomainModel();
        }
    }
}
