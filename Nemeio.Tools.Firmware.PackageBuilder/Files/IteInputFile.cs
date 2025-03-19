using System.IO;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Core.Files;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files
{
    internal class IteInputFile : InputFile
    {
        private const string IteFileExtension = ".bin";

        public IteInputFile(IFileSystem fileSystem, string filePath) 
            : base(fileSystem, filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (extension != IteFileExtension)
            {
                throw new InvalidDataException($"ITE binary file must be a {IteFileExtension}");
            }
        }
    }
}
