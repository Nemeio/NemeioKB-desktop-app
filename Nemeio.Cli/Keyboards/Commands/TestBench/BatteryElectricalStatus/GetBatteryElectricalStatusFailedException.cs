using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.BatteryElectricalStatus
{
    internal sealed class GetBatteryElectricalStatusFailedException : CommandFailedException
    {
        public GetBatteryElectricalStatusFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public GetBatteryElectricalStatusFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public GetBatteryElectricalStatusFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
