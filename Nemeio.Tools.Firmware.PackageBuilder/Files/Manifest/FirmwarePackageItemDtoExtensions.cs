using System;

namespace Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest
{
    public static class FirmwarePackageItemDtoExtensions
    {
        public static FirmwareInformation ToDomainModel(this FirmwarePackageItemDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            return new FirmwareInformation(dto.Version, dto.Filename);
        }
    }
}
