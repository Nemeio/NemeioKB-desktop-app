using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Add
{
    internal sealed class AddLayoutCommandFailedException : CommandFailedException
    {
        public AddLayoutCommandFailedException()
            : base(ApplicationExitCode.AddLayoutFailed) { }

        public AddLayoutCommandFailedException(string message)
            : base(ApplicationExitCode.AddLayoutFailed, message) { }

        public AddLayoutCommandFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.AddLayoutFailed, message, innerException) { }
    }
}
