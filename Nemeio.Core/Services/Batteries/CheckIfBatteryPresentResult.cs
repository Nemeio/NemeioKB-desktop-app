using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Services.TestBench;
using System;

namespace Nemeio.Core.Services.TestBench
{
    public class CheckIfBatteryPresentResult : TestBenchResultBase
    {
        public bool IsBatteryPresent { get; private set; }



        public CheckIfBatteryPresentResult(KeyboardErrorCode executionResult, bool isBatteryPresent) :
            base(executionResult)
        {
            IsBatteryPresent = isBatteryPresent;
        }
    }
}
