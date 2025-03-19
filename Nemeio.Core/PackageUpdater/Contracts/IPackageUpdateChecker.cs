using System.Threading.Tasks;
using Nemeio.Core.PackageUpdater.Informations;

namespace Nemeio.Core.PackageUpdater
{
    public interface IPackageUpdateChecker
    {
        Task<DownloadablePackageInformation> ApplicationNeedUpdateAsync();
    }
}
