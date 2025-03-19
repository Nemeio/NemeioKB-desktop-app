using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.FactoryReset
{
    internal sealed class FactoryResetFailedException : CommandFailedException
    {
        public FactoryResetFailedException()
            : base(ApplicationExitCode.FactoryResetFailed) { }

        public FactoryResetFailedException(string message)
            : base(ApplicationExitCode.FactoryResetFailed, message) { }

        public FactoryResetFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.FactoryResetFailed, message, innerException) { }
    }
}
