using System.IO;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Core.Files;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Files
{
    internal sealed class BmpInputFile : InputFile
    {
        private const string BmpFileExtension = ".bmp";

        public BmpInputFile(IFileSystem fileSystem, string filePath) 
            : base(fileSystem, filePath)
        {
            var fileExtension = _fileSystem.GetFileExtension(filePath);
            if (fileExtension != BmpFileExtension)
            {
                throw new InvalidDataException($"Image file must be a {BmpFileExtension}");
            }
        }
    }
}
