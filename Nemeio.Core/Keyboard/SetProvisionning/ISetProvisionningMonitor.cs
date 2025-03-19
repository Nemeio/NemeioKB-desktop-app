using Nemeio.Core.Services.Batteries;
using System.Net.NetworkInformation;

namespace Nemeio.Core.Keyboard.SetProvisionning
{
    public interface ISetProvisionningMonitor
    {
        GenericTestBenchResult SetProvisionning(string serial, PhysicalAddress mac);
    }
}
