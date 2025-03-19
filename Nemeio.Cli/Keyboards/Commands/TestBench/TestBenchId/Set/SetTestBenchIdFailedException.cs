using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Set
{
    internal sealed class SetTestBenchIdFailedException : CommandFailedException
    {
        public SetTestBenchIdFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public SetTestBenchIdFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public SetTestBenchIdFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
