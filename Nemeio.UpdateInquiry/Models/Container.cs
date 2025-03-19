using System.Collections.Generic;
using System.Linq;

namespace Nemeio.UpdateInquiry.Models
{
    public class Container
    {
        #region Properties
        public IList<Binary> Cpu { get; set; }
        public IList<Binary> Ble { get; set; }
        public IList<Binary> Ite { get; set; }
        public IList<Binary> Win { get; set; }
        public IList<Binary> Osx { get; set; }
        public IList<Binary> WinCli { get; set; }
        public IList<Binary> LinuxCli { get; set; }
        public IList<Binary> WinMiniInstaller { get; set; }
        public IList<Binary> OsxMiniInstaller { get; set; }
        public IList<Binary> IsoMiniInstaller { get; set; }
        public IList<Binary> Sbsfu { get; set; }
        public IList<Binary> ScratchInstaller { get; set; }
        public IList<Binary> Sfb { get; set; }
        public IList<Binary> NrfSecureBootloader { get; set; }
        public IList<Binary> NrfSettings { get; set; }
        public IList<Binary> NrfSoftDevice { get; set; }
        public IList<Binary> NrfPackageAppOnly { get; set; }
        public IList<Binary> NrfPackageComplete { get; set; }
        public IList<Binary> NrfScratchInstaller { get; set; }
        public IList<Binary> WinImageLoader { get; set; }
        public IList<Binary> LinuxImageLoader { get; set; }
        public IList<Binary> WinImagePackageBuilder { get; set; }
        public IList<Binary> LinuxImagePackageBuilder { get; set; }
        public IList<Binary> Package { get; set; }
        public IList<Binary> Package_zip { get; set; }


        #endregion

        #region Constructor

        public Container()
        {
            Cpu = new List<Binary>();
            Ble = new List<Binary>();
            Ite = new List<Binary>();
            Win = new List<Binary>();
            WinCli = new List<Binary>();
            LinuxCli = new List<Binary>();
            Osx = new List<Binary>();
            WinMiniInstaller = new List<Binary>();
            OsxMiniInstaller = new List<Binary>();
            IsoMiniInstaller = new List<Binary>();
            Sbsfu = new List<Binary>();
            ScratchInstaller = new List<Binary>();
            Sfb = new List<Binary>();
            NrfSecureBootloader = new List<Binary>();
            NrfSettings = new List<Binary>();
            NrfSoftDevice = new List<Binary>();
            NrfPackageAppOnly = new List<Binary>();
            NrfPackageComplete = new List<Binary>();
            NrfScratchInstaller = new List<Binary>();
            WinImageLoader = new List<Binary>();
            LinuxImageLoader = new List<Binary>();
            WinImagePackageBuilder = new List<Binary>();
            LinuxImagePackageBuilder = new List<Binary>();
            Package = new List<Binary>();
            Package_zip = new List<Binary>();
        }

        #endregion

        #region Methods

        public Binary GetLatestWindows(Platform platform) => Win.Where(x => x.Platform == platform).OrderBy(x => x.Version).Reverse().FirstOrDefault();
        public Binary GetLatestWindowsCli(Platform platform) => WinCli.Where(x => x.Platform == platform).OrderBy(x => x.Version).Reverse().FirstOrDefault();
        public Binary GetLatestLinuxCli(Platform platform) => LinuxCli.Where(x => x.Platform == platform).OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestOsx() => Osx.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestCpu() => Cpu.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestBle() => Ble.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestIte() => Ite.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestWinMiniInstaller() => WinMiniInstaller.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestOsxMiniInstaller() => OsxMiniInstaller.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestIsoMiniInstaller() => IsoMiniInstaller.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestSbsfu() => Sbsfu.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestScratchInstaller() => ScratchInstaller.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestSfb() => Sfb.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestNrfSecureBootloader() => NrfSecureBootloader.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestNrfSettings() => NrfSettings.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestNrfSoftDevice() => NrfSoftDevice.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestNrfPackageAppOnly() => NrfPackageAppOnly.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestNrfPackageComplete() => NrfPackageComplete.OrderBy(x => x.Version).Reverse().FirstOrDefault();
        public Binary GetLatestNrfScratchInstaller() => NrfScratchInstaller.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestWinImageLoader() => WinImageLoader.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        public Binary GetLatestLinuxImageLoader() => LinuxImageLoader.OrderBy(x => x.Version).Reverse().FirstOrDefault();
        public Binary GetLatestWinImagePackageBuilder() => WinImagePackageBuilder.OrderBy(x => x.Version).Reverse().FirstOrDefault();
        public Binary GetLatestLinuxImagePackageBuilder() => LinuxImagePackageBuilder.OrderBy(x => x.Version).Reverse().FirstOrDefault();
        public Binary GetLatestPackage() => Package.OrderBy(x => x.Version).Reverse().FirstOrDefault();
        public Binary GetLatestPackage_zip() => Package_zip.OrderBy(x => x.Version).Reverse().FirstOrDefault();

        #endregion
    }
}
