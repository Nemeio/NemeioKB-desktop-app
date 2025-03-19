using System.IO;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Core.Files;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files
{
    internal class ManifestInputFile : InputFile
    {
        private const string ManifestFileExtension = ".json";

        public ManifestInputFile(IFileSystem fileSystem, string filePath) 
            : base(fileSystem, filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (extension != ManifestFileExtension)
            {
                throw new InvalidDataException($"Manifest file must be a {ManifestFileExtension}");
            }
        }
    }
}
