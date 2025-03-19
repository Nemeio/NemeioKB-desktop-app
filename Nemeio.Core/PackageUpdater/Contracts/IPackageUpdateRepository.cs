using System.Threading.Tasks;
using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Core.PackageUpdater
{
    public interface IPackageUpdateRepository
    {
        Task<DownloadablePackageInformation> GetApplicationPackageInformationAsync();
    }
}
