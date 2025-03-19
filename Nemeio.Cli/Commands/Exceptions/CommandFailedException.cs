using System;
using Nemeio.Cli.Application;
using Nemeio.Tools.Core;

namespace Nemeio.Cli.Commands.Exceptions
{
    internal class CommandFailedException : ApplicationException<ApplicationExitCode>
    {
        public CommandFailedException(ApplicationExitCode exitCode)
            : base(exitCode) { }

        public CommandFailedException(ApplicationExitCode exitCode, string message) 
            : base(exitCode, message) { }

        public CommandFailedException(ApplicationExitCode exitCode, string message, Exception innerException) 
            : base(exitCode, message, innerException) { }
    }
}
