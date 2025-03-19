using System;
using System.Collections.Generic;

namespace Nemeio.Core.DataModels
{
    public enum KeyboardEventId
    {
        None = -1,
        FaultExceptionEvent = 0,
        AssertFailEvent = 1,
        TestEvent = 2,
    }

    public enum KeyboardExceptionEventTypeId
    {
        None = -1,
        NMI = 0,
        HardFault = 1,
        MemManageFault = 2,
        BusFault = 3,
        UsageFault = 4,
        DebugMon = 5
    }

    public class KeyboardFailure
    {
        public static int NumberOfRegistries { get; } = 13;

        public KeyboardEventId Id { get; }

        public UInt32[] Registries { get; }

        public UInt32 SP { get; }

        public UInt32 LR { get; }

        public UInt32 PC { get; }

        public UInt32 xPSR { get; }

        public KeyboardExceptionEventTypeId ExceptionType { get; }

        public List<string> Description { get; } = new List<string>();

        public KeyboardFailure(KeyboardEventId id, UInt32[] registries, UInt32 sp, UInt32 lr, UInt32 pc, UInt32 xpsr, KeyboardExceptionEventTypeId exceptionType = KeyboardExceptionEventTypeId.None)
        {
            Id = (KeyboardEventId)id;

            if (registries.Length != 13)
            {
                throw new ArgumentException("Registries number invalid");
            }
            Description = new List<string>();
            Registries = registries;
            SP = sp;
            LR = lr;
            PC = pc;
            xPSR = xpsr;
            ExceptionType = exceptionType;
        }

        public KeyboardFailure(KeyboardEventId id, List<string> description)
        {
            Id = (KeyboardEventId)id;
            Description.AddRange(description);
        }
    }
}
