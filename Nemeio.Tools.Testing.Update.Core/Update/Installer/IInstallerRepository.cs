using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Tools.Testing.Update.Core.Update.Environment;

namespace Nemeio.Tools.Testing.Update.Core.Update.Installer
{
    public interface IInstallerRepository
    {
        Task<IEnumerable<Installer>> GetInstallers(Uri serverUri, NemeioEnvironment environment);
    }
}
