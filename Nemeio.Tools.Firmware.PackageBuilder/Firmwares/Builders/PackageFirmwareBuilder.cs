using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Nemeio.Core;
using Nemeio.Core.FileSystem;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Tools.Firmware.PackageBuilder.Builders
{
    public class PackageFirmwareBuilder : IPackageFirmwareBuilder
    {
        private static readonly uint FirmwareTag = 0x1d1c1d1c;
        private static readonly uint InitMagicNumber = 0xb001ab1e;
        private static readonly uint FirmwareMagicNumber = 0x0b1eb1e0;

        private const int FormatVersion = 1;
        private const PackageCompressionType CompressionType = PackageCompressionType.GZip;
        private const int PackageHeaderLength = 41;
        private const int PackageFirmwareHeaderLength = 16;

        private readonly INrfManifestReader _nrfManifestReader;
        private readonly IFileSystem _fileSystem;

        public PackageFirmwareBuilder(INrfManifestReader nrfManifestReader, IFileSystem fileSystem)
        {
            _nrfManifestReader = nrfManifestReader ?? throw new ArgumentNullException(nameof(nrfManifestReader));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task<IPackageFirmware> CreatePackageAsync(FirmwareUpdateInformation manifest)
        {
            var nrfFirmware = await CreateBluetoothLEBinary(manifest.Informations[FirmwareUpdateModule.Nrf]);

            var package = new PackageFirmware()
            {
                GlobalHeader = CreatePackageHeader(),
                Stm32Header = CreatePackageFirmwareHeader(manifest.Informations[FirmwareUpdateModule.Cpu]),
                NrfHeader = CreatePackageFirmwareHeader(manifest.Informations[FirmwareUpdateModule.Nrf]),
                IteHeader = CreatePackageFirmwareHeader(manifest.Informations[FirmwareUpdateModule.Ite]),
                Stm32Firmware = CreateFirmwareBinary(manifest.Informations[FirmwareUpdateModule.Cpu]),
                NrfFirmware = nrfFirmware,
                IteFirmware = CreateFirmwareBinary(manifest.Informations[FirmwareUpdateModule.Ite]),
            };

            package.Stm32Header.Size = package.Stm32Firmware.Length;
            package.NrfHeader.Size = package.NrfFirmware.Length;
            package.IteHeader.Size = package.IteFirmware.Length;

            package.GlobalHeader.Size = ComputePackageSize(package);
            package.Stm32Header.Offset = ComputeStm32FirmwareOffset(package);
            package.NrfHeader.Offset = ComputeNrfFirmwareOffset(package);
            package.IteHeader.Offset = ComputeIteFirmwareOffset(package);

            return package;
        }

        private FirmwarePackageHeader CreatePackageHeader()
        {
            var packageHeader = new FirmwarePackageHeader()
            {
                Tag = FirmwareTag,
                FormatVersion = FormatVersion,
                Size = 0,
                Signature = ComputeSignature(),
            };

            return packageHeader;
        }

        private FirmwarePackageFirmwareHeader CreatePackageFirmwareHeader(EmbeddedPackageInformation packageUpdateInformation)
        {
            var packageFirmwareHeader = new FirmwarePackageFirmwareHeader()
            {
                CompressionType = CompressionType,
                MajorVersion = checked((byte)packageUpdateInformation.Version.Major),
                MinorVersion = checked((byte)packageUpdateInformation.Version.Minor),
                RevisionVersion = 0,
                BuildVersion = 0,
                Offset = 0,
                Size = 0,
            };

            return packageFirmwareHeader;
        }

        private byte[] CreateFirmwareBinary(EmbeddedPackageInformation packageUpdateInformation)
        {
            switch (CompressionType)
            {
                case PackageCompressionType.None:
                    return System.IO.File.ReadAllBytes(packageUpdateInformation.Path);

                case PackageCompressionType.GZip:
                    using (var input = System.IO.File.OpenRead(packageUpdateInformation.Path))
                    using (var compressStream = new MemoryStream())
                    {
                        using (var compressor = new GZipStream(compressStream, CompressionLevel.Optimal))
                        {
                            input.CopyTo(compressor);
                        }

                        return compressStream.ToArray();
                    }

                default:
                    throw new InvalidOperationException($"Compression type {CompressionType} not supported.");
            }
        }

        private async Task<byte[]> CreateBluetoothLEBinary(EmbeddedPackageInformation bluetoothLECompleted)
        {
            if (bluetoothLECompleted == null)
            {
                throw new ArgumentNullException(nameof(bluetoothLECompleted));
            }

            //  Create addons headers
            //  Documentation : https://adeneo-embedded.atlassian.net/wiki/spaces/BLDLCK/pages/1876230978/Package+de+mise+jour

            var bluetoothLEZipPath = bluetoothLECompleted.Path;

            //  Unzip file
            var destination = Path.Combine(
                Path.GetDirectoryName(bluetoothLEZipPath),
                Path.GetFileNameWithoutExtension(bluetoothLEZipPath)
            );

            //  By default remove file if already exists
            _fileSystem.RemoveFolderIfExists(destination);

            ZipFile.ExtractToDirectory(bluetoothLEZipPath, destination);

            //  Get manifest data
            const string manifestFileName = "manifest.json";

            var nrfManifestFilePath = Path.Combine(destination, manifestFileName);
            var nrfManifest = await _nrfManifestReader.ParseManifest(nrfManifestFilePath);

            //  Create headers
            var bluetoothLEData = await BuildNrfComponents(destination, nrfManifest);

            using (var compressStream = new MemoryStream())
            {
                using (var compressor = new GZipStream(compressStream, CompressionLevel.Optimal))
                {
                    compressor.Write(bluetoothLEData, 0, bluetoothLEData.Length);
                    compressor.Close();
                }

                return compressStream.ToArray();
            }
        }

        private async Task<byte[]> BuildNrfComponents(string destination, NrfManifest nrfManifest)
        {
            var bluetoothLEData = new byte[0];

            //  We add softDevice only if needed
            if (nrfManifest.SoftDevice != null)
            {
                var softDeviceComponent = await BuildComponent(destination, nrfManifest.SoftDevice, 0);
                var softDeviceData = softDeviceComponent.ToArray();

                bluetoothLEData = bluetoothLEData.Append(softDeviceData);
            }

            var applicationComponent = await BuildComponent(destination, nrfManifest.Application, bluetoothLEData.Length, true);
            var applicationData = applicationComponent.ToArray();

            bluetoothLEData = bluetoothLEData.Append(applicationData);

            return bluetoothLEData;
        }

        private async Task<byte[]> BuildComponent(string folderPath, NrfComponent component, int offset, bool lastOne = false)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            var init = await BuildFirmwareComponent(
                Path.Combine(folderPath, component.DatFile),
                InitMagicNumber,
                offset
            );

            var firmware = await BuildFirmwareComponent(
                Path.Combine(folderPath, component.BinFile),
                FirmwareMagicNumber,
                offset + init.Length,
                lastOne
            );

            return init
                .Concat(firmware)
                .ToArray();
        }

        private async Task<byte[]> BuildFirmwareComponent(string filePath, uint magicNumber, int offset, bool lastOne = false)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var exists = _fileSystem.FileExists(filePath);
            if (!exists)
            {
                throw new InvalidOperationException();
            }

            const int magicNumberSize = 4;
            const int sizeSize = 4;
            const int offsetSize = 4;
            const int headerSize = magicNumberSize + sizeSize + offsetSize;

            var fileContent = await _fileSystem.ReadByteArrayAsync(filePath);

            var result = new byte[] { };
            result = result.Append(BitConverter.GetBytes(magicNumber));
            result = result.Append(BitConverter.GetBytes(fileContent.Length));
            result = result.Append(BitConverter.GetBytes(lastOne ? 0 : offset + headerSize + fileContent.Length));
            result = result.Append(fileContent);

            return result;
        }

        private byte[] ComputeSignature()
        {
            // TODO BLDLCK-2817.
            return new byte[32];
        }

        private int ComputeStm32FirmwareOffset(PackageFirmware package)
        {
            return PackageHeaderLength +
                   3 * PackageFirmwareHeaderLength;
        }

        private int ComputeNrfFirmwareOffset(PackageFirmware package)
        {
            return ComputeStm32FirmwareOffset(package) +
                   package.Stm32Firmware.Length;
        }

        private int ComputeIteFirmwareOffset(PackageFirmware package)
        {
            return ComputeNrfFirmwareOffset(package) +
                   package.NrfFirmware.Length;
        }

        private int ComputePackageSize(PackageFirmware package)
        {
            return ComputeIteFirmwareOffset(package) +
                   package.IteFirmware.Length;
        }
    }
}
