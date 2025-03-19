using System.IO;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Core.Files;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files
{
    internal class NrfInputFile : InputFile
    {
        private const string NrfFileExtension = ".zip";

        public NrfInputFile(IFileSystem fileSystem, string filePath) 
            : base(fileSystem, filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (extension != NrfFileExtension)
            {
                throw new InvalidDataException($"NRF file must be a {NrfFileExtension}");
            }
        }
    }
}
