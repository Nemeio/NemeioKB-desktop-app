using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nemeio.Tools.Testing.Update.Core.Update.Software;

namespace Nemeio.Tools.Testing.Update.Application.Windows.Update.Software
{
    public class WinSoftwareExecutor : ISoftwareExecutor
    {
        private TimeSpan WaitingAliveTime = new TimeSpan(0, 0, 5);

        public Task<LaunchStatus> RunAsync(string path, string processName)
        {
            if (!File.Exists(path))
            {
                return Task.FromResult(LaunchStatus.Error);
            }

            return Task<LaunchStatus>.Factory.StartNew(() =>
            {
                try
                {
                    var msiProcess = new Process();
                    msiProcess.StartInfo.FileName = path;
                    var startSucceed = msiProcess.Start();

                    if (!startSucceed)
                    {
                        return LaunchStatus.Error;
                    }

                    Task.Delay(WaitingAliveTime).Wait();

                    var processes = Process.GetProcessesByName(processName);
                    if (processes.Length == 0)
                    {
                        return LaunchStatus.Error;
                    }

                    return LaunchStatus.Success;
                }
                catch
                {
                    return LaunchStatus.Error;
                }
            });
        }

        public Task<LaunchStatus> ForceQuit(string processName)
        {
            return Task<LaunchStatus>.Factory.StartNew(() => 
            {
                try
                {
                    var processes = Process.GetProcessesByName(processName);
                    if (processes.Length > 1)
                    {
                        return LaunchStatus.Error;
                    }

                    processes.First().Kill();

                    processes = Process.GetProcessesByName(processName);

                    return processes.Length > 1 ? LaunchStatus.Error : LaunchStatus.Success;
                }
                catch
                {
                    return LaunchStatus.Error;
                }
            });
        }
    }
}
