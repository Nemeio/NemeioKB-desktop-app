using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.List
{
    internal sealed class ListLayoutFailedException : CommandFailedException
    {
        public ListLayoutFailedException()
            : base(ApplicationExitCode.ListLayoutFailed) { }

        public ListLayoutFailedException(string message)
            : base(ApplicationExitCode.ListLayoutFailed, message) { }

        public ListLayoutFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.ListLayoutFailed, message, innerException) { }
    }
}
