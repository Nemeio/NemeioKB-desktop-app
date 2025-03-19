using System;
using System.Threading.Tasks;

namespace Nemeio.Tools.Testing.Update.Core.Update.Installer
{
    public interface IInstallerExecutor
    {
        Task<InstallationStatus> InstallAsync(string path, Version version);
        Task<InstallationStatus> UninstallAsync(string path, Version version);
    }
}
