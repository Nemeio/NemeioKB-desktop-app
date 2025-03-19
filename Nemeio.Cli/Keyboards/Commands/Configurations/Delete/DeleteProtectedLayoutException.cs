using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Delete
{
    internal sealed class DeleteProtectedLayoutException : CommandFailedException
    {
        public DeleteProtectedLayoutException()
            : base(ApplicationExitCode.DeleteProtectedLayout) { }

        public DeleteProtectedLayoutException(string message)
            : base(ApplicationExitCode.DeleteProtectedLayout, message) { }

        public DeleteProtectedLayoutException(string message, Exception innerException)
            : base(ApplicationExitCode.DeleteProtectedLayout, message, innerException) { }
    }
}
