using System;
using Nemeio.Core.Applications.Manifest;

namespace Nemeio.Presentation.PackageUpdater.Firmware
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
