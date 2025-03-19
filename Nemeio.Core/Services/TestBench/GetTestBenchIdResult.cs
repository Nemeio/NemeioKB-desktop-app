using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Services.TestBench;
using System;

namespace Nemeio.Core.Services.TestBench
{
    public class GetTestBenchIdResult : TestBenchResultBase
    {
        public string TestBenchId { get; private set; }



        public GetTestBenchIdResult(KeyboardErrorCode executionResult, string testBenchId) :
            base(executionResult)
        {
            TestBenchId = testBenchId;
        }
    }
}
