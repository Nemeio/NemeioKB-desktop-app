using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Configurations
{
    internal sealed class LayoutNotFoundException : CommandFailedException
    {
        public LayoutNotFoundException()
            : base(ApplicationExitCode.LayoutNotFound) { }

        public LayoutNotFoundException(string message)
            : base(ApplicationExitCode.LayoutNotFound, message) { }

        public LayoutNotFoundException(string message, Exception innerException)
            : base(ApplicationExitCode.LayoutNotFound, message, innerException) { }
    }
}
