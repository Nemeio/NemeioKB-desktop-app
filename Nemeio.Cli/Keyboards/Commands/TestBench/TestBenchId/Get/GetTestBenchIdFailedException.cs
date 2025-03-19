using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.TestBenchId.Get
{
    internal sealed class GetTestBenchIdFailedException : CommandFailedException
    {
        public GetTestBenchIdFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public GetTestBenchIdFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public GetTestBenchIdFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
