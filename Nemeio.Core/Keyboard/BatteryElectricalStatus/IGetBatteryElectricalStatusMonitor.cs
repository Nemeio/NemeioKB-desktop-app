using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.BatteryElectricalStatus
{
    public interface IGetBatteryElectricalStatusMonitor
    {
        Services.Batteries.BatteryElectricalState GetBatteryElectricalStatus();
    }
}
