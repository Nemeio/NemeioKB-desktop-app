using System.Collections.Generic;
using Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest;

namespace Nemeio.Tools.Firmware.PackageBuilder.Builders
{
    public enum FirmwareUpdateModule
    {
        Cpu,
        Nrf,
        Ite
    }

    public class FirmwareUpdateInformation
    {
        public IDictionary<FirmwareUpdateModule, EmbeddedPackageInformation> Informations { get; private set; }

        public FirmwareUpdateInformation()
        {
            Informations = new Dictionary<FirmwareUpdateModule, EmbeddedPackageInformation>();
        }

        public void AddManifest(FirmwareManifest manifest)
        {
            AddModule(FirmwareUpdateModule.Cpu, manifest.Cpu);
            AddModule(FirmwareUpdateModule.Nrf, manifest.BluetoothLE);
            AddModule(FirmwareUpdateModule.Ite, manifest.Ite);
        }

        public void AddModule(FirmwareUpdateModule module, FirmwareInformation information)
        {
            var embbededPackageInformation = new EmbeddedPackageInformation(information);

            AddModule(module, embbededPackageInformation);
        }

        public void AddModule(FirmwareUpdateModule module, EmbeddedPackageInformation information)
        {
            Informations.Add(module, information);
        }
    }
}
