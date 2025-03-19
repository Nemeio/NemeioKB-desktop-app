using System;
using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.Keyboard.Battery
{
    public interface IBatteryHolder
    {
        BatteryInformation Battery { get; }

        event EventHandler OnBatteryLevelChanged;
    }
}
