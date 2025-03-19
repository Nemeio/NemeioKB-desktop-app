using System;
using System.Threading.Tasks;

namespace Nemeio.Tools.Testing.Update.Core.Update.Installer
{
    public interface IInstallerCheckerVersion
    {
        Task<bool> IsVersion(Version version);
    }
}
