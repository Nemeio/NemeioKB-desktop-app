using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Services.TestBench;
using System;

namespace Nemeio.Core.Services.Batteries
{
    public class GenericTestBenchResult : TestBenchResultBase
    {
        public GenericTestBenchResult(KeyboardErrorCode executionResult) :
            base(executionResult)
        {

        }
    }
}
