using Nemeio.Core.Keyboard.Communication.Errors;
using System;

namespace Nemeio.Core.Services.TestBench
{
    public abstract class TestBenchResultBase
    {
        public KeyboardErrorCode ExecutionResult { get; private set; }
        public string ErrorText { get { return ExecutionResult.ToString(); } }
        protected TestBenchResultBase(KeyboardErrorCode executionResult)
        {
            ExecutionResult = executionResult;
        }
    }
}
