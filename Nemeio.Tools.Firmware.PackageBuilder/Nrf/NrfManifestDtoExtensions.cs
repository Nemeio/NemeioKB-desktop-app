namespace Nemeio.Tools.Firmware.PackageBuilder
{
    public static class NrfManifestDtoExtensions
    {
        public static NrfManifest ToDomainModel(this NrfManifestDto dto)
        {
            var application = new NrfComponent(
                dto.Manifest.Application.BinFile,
                dto.Manifest.Application.DatFile
            );

            NrfComponent softDevice = null;

            if (dto.Manifest.SoftDevice != null)
            {
                softDevice = new NrfComponent(
                    dto.Manifest.SoftDevice.BinFile,
                    dto.Manifest.SoftDevice.DatFile
                );
            }

            return new NrfManifest(application, softDevice);
        }
    }
}
