using System.IO;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Core.Files;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files
{
    internal class StmInputFile : InputFile
    {
        private const string StmFileExtension = ".sfb";

        public StmInputFile(IFileSystem fileSystem, string filePath) 
            : base(fileSystem, filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (extension != StmFileExtension)
            {
                throw new InvalidDataException($"STM binary file must be a {StmFileExtension}");
            }
        }
    }
}
