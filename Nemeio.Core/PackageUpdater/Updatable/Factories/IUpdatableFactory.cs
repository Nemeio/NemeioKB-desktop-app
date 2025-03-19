using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Core.PackageUpdater.Updatable.Factories
{
    public interface IUpdatableFactory
    {
        IUpdatable CreateUpdatableSoftware(DownloadablePackageInformation packageInfo);
        IUpdatable CreateUpdatableKeyboard(FirmwareUpdatableNemeioProxy proxy);
    }
}
