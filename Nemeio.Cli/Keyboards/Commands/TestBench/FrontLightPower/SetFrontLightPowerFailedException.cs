using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.FrontLightPower
{
    internal sealed class SetFrontLightPowerFailedException : CommandFailedException
    {
        public SetFrontLightPowerFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public SetFrontLightPowerFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public SetFrontLightPowerFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
