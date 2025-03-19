using System;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest
{
    public static class FirmwarePackageManifestDtoExtensions
    {
        public static FirmwareManifest ToDomainModel(this FirmwarePackageManifestDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            return new FirmwareManifest()
            {
                Cpu = dto.Cpu.ToDomainModel(),
                BluetoothLE = dto.BluetootLE.ToDomainModel(),
                Ite = dto.Ite.ToDomainModel()
            };
        }
    }
}
