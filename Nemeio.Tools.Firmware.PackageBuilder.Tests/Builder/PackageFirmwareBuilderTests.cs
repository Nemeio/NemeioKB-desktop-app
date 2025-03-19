using System;
using System.IO;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Firmware.PackageBuilder;
using Nemeio.Tools.Firmware.PackageBuilder.Builders;
using Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest;
using NUnit.Framework;

namespace Nemeio.Presentation.Test.PackageUpdater
{
    [TestFixture]
    public class PackageFirmwareBuilderTests
    {
        public async Task WritePackage_Success()
        {
            var fileSystem = new FileSystem();
            var nrfManifestReader = new NrfManifestReader(fileSystem);

            var builder = new PackageFirmwareBuilder(nrfManifestReader, fileSystem);

            string path = Path.Combine("Firmwares/package.bin");

            var manifest = new FirmwareManifest()
            {
                Cpu = new FirmwareInformation(new Version("1.0.0"), "cpu.bin"),
                BluetoothLE = new FirmwareInformation(new Version("1.0.0"), "ble.bin"),
                Ite = new FirmwareInformation(new Version("1.0.0"), "ite.bin")
            };

            var information = new FirmwareUpdateInformation();
            information.AddManifest(manifest);

            var packageFirmware = await builder.CreatePackageAsync(information);

            using (Stream stream = System.IO.File.Create(path))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                packageFirmware.Write(writer);
            }
        }
    }
}
