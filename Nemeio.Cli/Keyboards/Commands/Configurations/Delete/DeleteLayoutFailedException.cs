using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Delete
{
    internal sealed class DeleteLayoutFailedException : CommandFailedException
    {
        public DeleteLayoutFailedException()
            : base(ApplicationExitCode.DeleteLayoutFailed) { }

        public DeleteLayoutFailedException(string message)
            : base(ApplicationExitCode.DeleteLayoutFailed, message) { }

        public DeleteLayoutFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.DeleteLayoutFailed, message, innerException) { }
    }
}
