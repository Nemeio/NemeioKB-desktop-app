using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Change
{
    internal sealed class ChangeListenerTimeoutException : CommandFailedException
    {
        public ChangeListenerTimeoutException()
            : base(ApplicationExitCode.ChangeLayoutFailed) { }

        public ChangeListenerTimeoutException(string message)
            : base(ApplicationExitCode.ChangeLayoutFailed, message) { }

        public ChangeListenerTimeoutException(string message, Exception innerException)
            : base(ApplicationExitCode.ChangeLayoutFailed, message, innerException) { }
    }
}
