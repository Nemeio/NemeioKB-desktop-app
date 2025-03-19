using System;
using System.IO;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Tools.Firmware.PackageBuilder.Firmwares.Writer
{
    internal class FirmwareWriter : IFirmwareWriter
    {
        private const string DefaultOutputFileName = "package.bin";

        private readonly IFileSystem _fileSystem;

        public FirmwareWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task WriteOnDisk(IPackageFirmware package, string outputFilePath = null)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            var filePath = string.Empty;

            if (!string.IsNullOrEmpty(outputFilePath))
            {
                var folderPath = Path.GetDirectoryName(outputFilePath);
                if (!_fileSystem.DirectoryExists(folderPath))
                {
                    throw new InvalidOperationException($"Folder at path <{folderPath}> doesn't exists");
                }

                var fileName = Path.GetFileName(outputFilePath);
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new InvalidOperationException($"Path <{outputFilePath}> must contains file name");
                }

                filePath = outputFilePath;
            }
            else
            {
                filePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), DefaultOutputFileName);
            }

            await _fileSystem.WriteAsync(filePath, package.ToByteArray());
        }
    }
}
