using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Set
{
    internal sealed class SetTestBenchIdInput
    {
        public string TestId { get; private set; }
        public SetTestBenchIdInput(string testId)
        {
            TestId = testId;
        }
    }
}