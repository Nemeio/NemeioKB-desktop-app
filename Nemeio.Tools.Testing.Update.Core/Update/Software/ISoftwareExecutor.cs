using System.Threading.Tasks;

namespace Nemeio.Tools.Testing.Update.Core.Update.Software
{
    public interface ISoftwareExecutor
    {
        Task<LaunchStatus> RunAsync(string path, string processName);
        Task<LaunchStatus> ForceQuit(string processName);
    }
}
