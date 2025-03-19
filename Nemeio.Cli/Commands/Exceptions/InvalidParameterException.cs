using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Commands.Exceptions
{
    internal sealed class InvalidParameterException : CommandFailedException
    {
        public InvalidParameterException()
            : base(ApplicationExitCode.InvalidParameter) { }

        public InvalidParameterException(string message)
            : base(ApplicationExitCode.InvalidParameter, message) { }

        public InvalidParameterException(string message, Exception innerException)
            : base(ApplicationExitCode.InvalidParameter, message, innerException) { }
    }
}
