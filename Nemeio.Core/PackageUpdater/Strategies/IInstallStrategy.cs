using System.Threading.Tasks;

namespace Nemeio.Core.PackageUpdater.Strategies
{
    public interface IInstallStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InstallFailedException">Installation can fail</exception>
        Task InstallAsync();
    }
}
