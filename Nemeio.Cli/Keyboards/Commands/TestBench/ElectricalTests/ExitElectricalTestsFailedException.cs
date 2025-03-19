using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.ElectricalTests
{
    internal sealed class ExitElectricalTestsFailedException : CommandFailedException
    {
        public ExitElectricalTestsFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public ExitElectricalTestsFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public ExitElectricalTestsFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
