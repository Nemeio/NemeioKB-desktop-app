using System;
using System.Threading.Tasks;
using Nemeio.Core.Keyboard.Updates;

namespace Nemeio.Core.PackageUpdater.Firmware
{
    public class PackageFirmwareInstaller : IPackageFirmwareInstaller
    {
        private readonly IFirmware _firmware;

        public PackageFirmwareInstaller(IFirmware firmware)
        {
            _firmware = firmware ?? throw new ArgumentNullException(nameof(firmware));
        }

        public async Task InstallAsync(IUpdateHolder updater)
        {
            //  Because the keyboard never anwser
            //  We don't want to wait this task
            await Task.Run(async () => await updater.UpdateAsync(_firmware));
        }
    }
}