using System;

namespace Nemeio.Core.Services.Batteries
{
    public class BatteryInformation
    {
        public BatteryLevel Level { get; private set; }

        public ushort RemainingCapacity { get; private set; }

        public BatteryTime TimeToFull { get; private set; }

        public BatteryTime TimeToEmpty { get; private set; }

        public ushort Cycles { get; private set; }

        public byte Age { get; private set; }

        public BatteryInformation(BatteryLevel level, ushort remainingCapacity, BatteryTime timeToFull, BatteryTime timeToEmpty, ushort cycles, byte age)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            RemainingCapacity = remainingCapacity;
            TimeToFull = timeToFull;
            TimeToEmpty = timeToEmpty;
            Cycles = cycles;
            Age = age;
        }
    }
}
