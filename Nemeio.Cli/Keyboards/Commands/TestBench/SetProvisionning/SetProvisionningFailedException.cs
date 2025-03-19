using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.SetProvisionning
{
    internal sealed class SetProvisionningFailedException : CommandFailedException
    {
        public SetProvisionningFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public SetProvisionningFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public SetProvisionningFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
