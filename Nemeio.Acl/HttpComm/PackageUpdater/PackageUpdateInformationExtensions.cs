using System;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Acl.HttpComm.PackageUpdater
{
    internal static class PackageUpdateInformationExtensions
    {
        public static DownloadablePackageInformation ToDomainModel(this PackageUpdateInDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            return new DownloadablePackageInformation(
                dto.Version,
                dto.Checksum,
                dto.Url
            );
        }

        public static PackageUpdateInDto ToDto(this DownloadablePackageInformation model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return new PackageUpdateInDto()
            {
                Checksum = model.Checksum,
                Version = model.Version,
                Url = model.Url,
            };
        }
    }
}
