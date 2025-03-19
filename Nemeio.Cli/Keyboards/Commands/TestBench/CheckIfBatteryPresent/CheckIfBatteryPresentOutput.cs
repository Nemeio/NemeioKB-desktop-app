using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.CheckIfBatteryPresent
{
    internal sealed class CheckIfBatteryPresentOutput
    {
        public string TestId { get; private set; }
        public CheckIfBatteryPresentOutput(string testId)
        {
            TestId = testId;
        }
    }
}