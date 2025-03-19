using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Get
{
    internal sealed class GetTestBenchIdOutput
    {
        public string TestId { get; private set; }
        public GetTestBenchIdOutput(string testId)
        {
            TestId = testId;
        }
    }
}