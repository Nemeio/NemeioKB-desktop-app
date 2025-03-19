using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.CheckComponents
{
    internal sealed class CheckComponentsFailedException : CommandFailedException
    {
        public CheckComponentsFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public CheckComponentsFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public CheckComponentsFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
