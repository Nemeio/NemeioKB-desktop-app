using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Core.PackageUpdater.Strategies
{
    public interface IInstallStrategyFactory
    {
        IInstallStrategy CreateFirmwareStrategy(FirmwareUpdatableNemeioProxy proxy);
        IInstallStrategy CreateApplicationStrategy(DownloadablePackageInformation packageInfo);
    }
}
