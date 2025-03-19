using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Nemeio.UpdateInquiry.Dto.Out;
using Nemeio.UpdateInquiry.Models;
using Nemeio.UpdateInquiry.Repositories;

namespace Nemeio.UpdateInquiry.Builders
{
    public enum UpdateEnvironment
    {
        Develop,
        Testing,
        Master,
        UpdateTesting
    }

    public class UpdateBuilder
    {
        private UpdateEnvironment _environment;
        private IUpdateRepository _updateRepository;

        public UpdateBuilder(UpdateEnvironment environment, IConfigurationRoot configurationRoot)
        {
            _environment = environment;
            _updateRepository = new BlobStorageUpdateRepository(configurationRoot);
        }

        public UpdateOutDto Build()
        {
            var container = _updateRepository.GetContainerByEnvironment(_environment);
            var outDto = new UpdateOutDto()
            {
                Cpu = EmbeddedSoftwareOutDto.FromBinary(container.GetLatestCpu()),
                ScratchInstaller = EmbeddedSoftwareOutDto.FromBinary(container.GetLatestScratchInstaller()),
                Ble = EmbeddedSoftwareOutDto.FromBinary(container.GetLatestBle()),
                BluetoothLEComplete = EmbeddedSoftwareOutDto.FromBinary(container.GetLatestNrfPackageComplete()),
                Ite = EmbeddedSoftwareOutDto.FromBinary(container.GetLatestIte()),
                Windows = OperatingSystemOutDto.FromBinaries(new List<Binary>() { container.GetLatestWindows(Platform.x86), container.GetLatestWindows(Platform.x64) }),
                Osx = OperatingSystemOutDto.FromBinaries(new List<Binary>() { container.GetLatestOsx() }),
                IsoMiniInstaller = SoftwareOutDto.FromBinary(container.GetLatestIsoMiniInstaller()),
                Sfb = SoftwareOutDto.FromBinary(container.GetLatestSfb()),
                NrfScratchInstaller = EmbeddedSoftwareOutDto.FromBinary(container.GetLatestNrfScratchInstaller()),
                CliWindows = SoftwareOutDto.FromBinary(container.GetLatestWindowsCli(Platform.x64)),
                CliLinux = SoftwareOutDto.FromBinary(container.GetLatestLinuxCli(Platform.x64)),
                WindowsImageLoader = SoftwareOutDto.FromBinary(container.GetLatestWinImageLoader()),
                LinuxImageLoader = SoftwareOutDto.FromBinary(container.GetLatestLinuxImageLoader()),
                WindowsImagePackageBuilder = SoftwareOutDto.FromBinary(container.GetLatestWinImagePackageBuilder()),
                LinuxPackageBuilder = SoftwareOutDto.FromBinary(container.GetLatestLinuxImagePackageBuilder()),
                Package = SoftwareOutDto.FromBinary(container.GetLatestPackage()),
                PackageZip = SoftwareOutDto.FromBinary(container.GetLatestPackage_zip()),
            };
            return outDto;
        }
    }
}
