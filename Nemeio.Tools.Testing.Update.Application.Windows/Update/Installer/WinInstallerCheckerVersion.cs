using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Nemeio.Tools.Testing.Update.Core.Update.Installer;

namespace Nemeio.Tools.Testing.Update.Application.Windows.Update.Installer
{
    public class WinInstallerCheckerVersion : IInstallerCheckerVersion
    {
        public Task<bool> IsVersion(Version version)
        {
            const string NemeioExecutablePath = @"C:\Program Files\Nemeio\Nemeio.exe";

            try
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(NemeioExecutablePath);
                var executableVersion = versionInfo.FileVersion;

                var isEqual = version.ToString() == executableVersion;

                return Task.FromResult(isEqual);
            }
            catch (FileNotFoundException)
            {
                return Task.FromResult(false);
            }
        }
    }
}
