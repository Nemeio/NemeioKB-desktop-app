using System;
using Nemeio.Cli.Application;

namespace Nemeio.Cli.Commands.Exceptions
{
    internal sealed class KeyboardDisconnectedException : CommandFailedException
    {
        public KeyboardDisconnectedException()
            : base(ApplicationExitCode.KeyboardDisconnected) { }

        public KeyboardDisconnectedException(string message)
            : base(ApplicationExitCode.KeyboardDisconnected, message) { }

        public KeyboardDisconnectedException(string message, Exception innerException)
            : base(ApplicationExitCode.KeyboardDisconnected, message, innerException) { }
    }
}
