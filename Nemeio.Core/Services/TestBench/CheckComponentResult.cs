using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Services.TestBench;
using System;

namespace Nemeio.Core.Services.TestBench
{
    public class CheckComponentResult : TestBenchResultBase
    {
        public CheckComponentResult(KeyboardErrorCode executionResult) : base(executionResult)
        {
        }
    }
}
