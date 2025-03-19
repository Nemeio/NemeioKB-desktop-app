using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.ClearScreen
{
    internal sealed class ClearScreenFailedException : CommandFailedException
    {
        public ClearScreenFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public ClearScreenFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public ClearScreenFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
