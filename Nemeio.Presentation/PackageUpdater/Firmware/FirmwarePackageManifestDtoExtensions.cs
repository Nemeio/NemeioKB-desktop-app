using System;
using Nemeio.Core.Applications.Manifest;

namespace Nemeio.Presentation.PackageUpdater.Firmware
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
