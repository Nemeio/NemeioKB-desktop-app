using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.Battery
{
    public interface IBatteryMonitor
    {
        BatteryInformation AskBattery();
    }
}
