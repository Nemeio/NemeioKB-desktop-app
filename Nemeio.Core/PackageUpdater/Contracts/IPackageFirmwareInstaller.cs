using System.Threading.Tasks;
using Nemeio.Core.Keyboard.Updates;

namespace Nemeio.Core.PackageUpdater
{
    public interface IPackageFirmwareInstaller
    {
        Task InstallAsync(IUpdateHolder nemeioUpdater);
    }
}