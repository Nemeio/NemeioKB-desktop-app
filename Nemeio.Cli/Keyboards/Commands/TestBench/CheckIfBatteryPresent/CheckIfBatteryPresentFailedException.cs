using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.CheckIfBatteryPresent
{
    internal sealed class CheckIfBatteryPresentFailedException : CommandFailedException
    {
        public CheckIfBatteryPresentFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public CheckIfBatteryPresentFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public CheckIfBatteryPresentFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.GetParametersFailed, message, innerException) { }
    }
}
