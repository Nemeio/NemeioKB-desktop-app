using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Apply
{
    internal sealed class ApplyLayoutFailedException : CommandFailedException
    {
        public ApplyLayoutFailedException()
            : base(ApplicationExitCode.ApplyLayoutFailed) { }

        public ApplyLayoutFailedException(string message)
            : base(ApplicationExitCode.ApplyLayoutFailed, message) { }

        public ApplyLayoutFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.ApplyLayoutFailed, message, innerException) { }
    }
}
