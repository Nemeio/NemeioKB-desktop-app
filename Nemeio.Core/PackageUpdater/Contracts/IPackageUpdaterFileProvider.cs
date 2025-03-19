using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Core.PackageUpdater
{
    public interface IPackageUpdaterFileProvider
    {
        string GetUpdateFilePath(PackageInformation package);
        string GetFilePath(PackageInformation package);
    }
}
