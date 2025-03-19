using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Nemeio.Tools.Testing.Update.Core.Update.Installer;

namespace Nemeio.Tools.Testing.Update.Application.Windows.Update.Installer
{
    internal class WinInstallerExecutor : IInstallerExecutor
    {
        const int SuccessExitCode = 0;

        public Task<InstallationStatus> InstallAsync(string path, Version version)
        {
            if (!File.Exists(path))
            {
                return Task.FromResult(InstallationStatus.Error);
            }

            return Task<InstallationStatus>.Factory.StartNew(() => 
            {
                var installerFolderPath = Path.GetDirectoryName(path);
                var installerLogPath = Path.Combine(installerFolderPath, $"installer.{version}.log");

                try
                {
                    var msiProcess = new Process();
                    msiProcess.StartInfo.UseShellExecute = false;
                    msiProcess.StartInfo.RedirectStandardOutput = true;
                    msiProcess.StartInfo.FileName = "msiexec.exe";
                    msiProcess.StartInfo.Arguments = $"/i \"{path}\" /qn /l*v \"{installerLogPath}\"";
                    msiProcess.Start();
                    msiProcess.WaitForExit();

                    return msiProcess.ExitCode == SuccessExitCode ? InstallationStatus.Succeed : InstallationStatus.Error;
                }
                catch
                {
                    return InstallationStatus.Error;
                }
            });
        }

        public Task<InstallationStatus> UninstallAsync(string path, Version version)
        {
            if (!File.Exists(path))
            {
                return Task.FromResult(InstallationStatus.Error);
            }

            return Task<InstallationStatus>.Factory.StartNew(() =>
            {
                var installerFolderPath = Path.GetDirectoryName(path);
                var installerLogPath = Path.Combine(installerFolderPath, $"uninstaller.{version}.log");

                try
                {
                    var msiProcess = new Process();
                    msiProcess.StartInfo.UseShellExecute = false;
                    msiProcess.StartInfo.RedirectStandardOutput = true;
                    msiProcess.StartInfo.FileName = "msiexec.exe";
                    msiProcess.StartInfo.Arguments = $"/uninstall \"{path}\" /qn /l*v \"{installerLogPath}\"";
                    msiProcess.Start();
                    msiProcess.WaitForExit();

                    return msiProcess.ExitCode == SuccessExitCode ? InstallationStatus.Succeed : InstallationStatus.Error;
                }
                catch
                {
                    return InstallationStatus.Error;
                }
            });
        }
    }
}
