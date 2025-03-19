using Nemeio.Core.Services.TestBench;

namespace Nemeio.Core.Keyboard.CheckIfBatteryPresent
{
    public interface ICheckIfBatteryPresentMonitor
    {
        CheckIfBatteryPresentResult CheckIfBatteryPresent();
    }
}
