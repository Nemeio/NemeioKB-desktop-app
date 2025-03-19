using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Parameters.Get
{
    internal class GetParametersFailedException : CommandFailedException
    {
        public GetParametersFailedException()
            : base(ApplicationExitCode.GetParametersFailed) { }

        public GetParametersFailedException(string message)
            : base(ApplicationExitCode.GetParametersFailed, message) { }

        public GetParametersFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.GetParametersFailed, message, innerException) { }
    }
}
