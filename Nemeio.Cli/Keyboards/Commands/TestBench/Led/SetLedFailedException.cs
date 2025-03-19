using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.Led
{
    internal sealed class SetLedFailedException : CommandFailedException
    {
        public SetLedFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public SetLedFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public SetLedFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
