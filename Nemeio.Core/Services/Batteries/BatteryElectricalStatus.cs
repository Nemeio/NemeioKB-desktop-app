using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Services.TestBench;
using System;

namespace Nemeio.Core.Services.Batteries
{
    public class BatteryElectricalState : TestBenchResultBase
    {
        public short Current { get; private set; }

        public ushort Voltage { get; private set; }

        public BatteryElectricalState(KeyboardErrorCode executionResult, short current, ushort voltage) :
            base(executionResult)
        {
            Current = current;
            Voltage = voltage;
        }
    }
}
