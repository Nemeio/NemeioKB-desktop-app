using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Update
{
    internal sealed class UpdateKeyboardFailedException : CommandFailedException
    {
        public UpdateKeyboardFailedException()
            : base(ApplicationExitCode.UpdateKeyboardFailed) { }

        public UpdateKeyboardFailedException(string message)
            : base(ApplicationExitCode.UpdateKeyboardFailed, message) { }

        public UpdateKeyboardFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.UpdateKeyboardFailed, message, innerException) { }
    }
}
