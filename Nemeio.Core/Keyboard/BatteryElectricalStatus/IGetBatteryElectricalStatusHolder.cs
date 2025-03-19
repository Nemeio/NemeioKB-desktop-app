using System;
using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.BatteryElectricalStatus
{
    public interface IGetBatteryElectricalStatusHolder
    {
        BatteryInformation Battery { get; }

        event EventHandler OnBatteryLevelChanged;
    }
}
