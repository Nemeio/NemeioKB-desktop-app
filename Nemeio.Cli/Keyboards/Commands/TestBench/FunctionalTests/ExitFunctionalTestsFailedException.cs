using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.FunctionalTests
{
    internal sealed class ExitFunctionalTestsFailedException : CommandFailedException
    {
        public ExitFunctionalTestsFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public ExitFunctionalTestsFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public ExitFunctionalTestsFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
