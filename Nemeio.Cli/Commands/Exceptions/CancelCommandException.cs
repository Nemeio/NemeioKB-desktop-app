using System;
using Nemeio.Cli.Application;

namespace Nemeio.Cli.Commands.Exceptions
{
    internal sealed class CancelCommandException : CommandFailedException
    {
        public CancelCommandException()
            : base(ApplicationExitCode.CanceledCommand) { }

        public CancelCommandException(string message)
            : base(ApplicationExitCode.CanceledCommand, message) { }

        public CancelCommandException(string message, Exception innerException)
            : base(ApplicationExitCode.CanceledCommand, message, innerException) { }
    }
}
