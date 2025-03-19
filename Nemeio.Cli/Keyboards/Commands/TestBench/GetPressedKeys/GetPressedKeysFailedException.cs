using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.PressedKeys
{
    internal sealed class GetPressedKeysFailedException : CommandFailedException
    {
        public GetPressedKeysFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public GetPressedKeysFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public GetPressedKeysFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
