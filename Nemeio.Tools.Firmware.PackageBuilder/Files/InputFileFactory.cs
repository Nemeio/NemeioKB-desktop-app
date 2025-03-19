using System;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Core.Files;
using Nemeio.Tools.Firmware.PackageBuilder.Builders;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files
{
    internal class InputFileFactory : IInputFileFactory
    {
        private IFileSystem _fileSystem;

        public InputFileFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(FileSystem));
        }

        public InputFile CreateManifestInputFile(string filePath)
        {
            var inputFile = new ManifestInputFile(_fileSystem, filePath);

            return inputFile;
        }

        public InputFile CreateFirmwareInputFile(FirmwareUpdateModule module, string filePath)
        {
            switch (module)
            {
                case FirmwareUpdateModule.Cpu:
                    return CreateStmInputFile(filePath);
                case FirmwareUpdateModule.Nrf:
                    return CreateNrfInputFile(filePath);
                case FirmwareUpdateModule.Ite:
                    return CreateIteInputFile(filePath);
                default:
                    throw new InvalidOperationException($"Unknow module <{module}>");
            }
        }

        private InputFile CreateStmInputFile(string filePath)
        {
            var inputFile = new StmInputFile(_fileSystem, filePath);

            return inputFile;
        }

        private InputFile CreateNrfInputFile(string filePath)
        {
            var inputFile = new NrfInputFile(_fileSystem, filePath);

            return inputFile;
        }

        private InputFile CreateIteInputFile(string filePath)
        {
            var inputFile = new IteInputFile(_fileSystem, filePath);

            return inputFile;
        }
    }
}
