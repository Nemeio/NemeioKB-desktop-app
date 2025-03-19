using Nemeio.Core.Services.TestBench;

namespace Nemeio.Core.Keyboard.SetLed
{
    public interface ICheckComponentsMonitor
    {
        CheckComponentResult CheckComponent(byte componentId);
    }
}
