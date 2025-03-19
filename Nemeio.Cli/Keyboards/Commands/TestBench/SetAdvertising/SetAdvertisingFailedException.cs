using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.SetAdvertising
{
    internal sealed class SetAdvertisingFailedException : CommandFailedException
    {
        public SetAdvertisingFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public SetAdvertisingFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public SetAdvertisingFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
