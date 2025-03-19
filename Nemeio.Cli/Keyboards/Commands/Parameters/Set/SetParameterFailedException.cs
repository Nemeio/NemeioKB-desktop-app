using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Parameters.Set
{
    internal sealed class SetParameterFailedException : CommandFailedException
    {
        public SetParameterFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public SetParameterFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public SetParameterFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
