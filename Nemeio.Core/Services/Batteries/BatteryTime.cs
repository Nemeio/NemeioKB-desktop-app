using System;

namespace Nemeio.Core.Services.Batteries
{
    public class BatteryTime
    {
        public const uint StandByValue = 0xFFFFFFFF;

        public bool StandBy { get; private set; }

        public TimeSpan Interval { get; private set; }

        public BatteryTime(uint value)
        {
            StandBy = value == StandByValue;
            Interval = TimeSpan.FromSeconds(value);
        }
    }
}
