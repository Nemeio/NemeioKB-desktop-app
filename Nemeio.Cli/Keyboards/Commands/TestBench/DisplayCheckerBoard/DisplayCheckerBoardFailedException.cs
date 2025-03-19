using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.DisplayCheckerBoard
{
    internal sealed class DisplayCheckerBoardFailedException : CommandFailedException
    {
        public DisplayCheckerBoardFailedException()
            : base(ApplicationExitCode.SetParametersFailed) { }

        public DisplayCheckerBoardFailedException(string message)
            : base(ApplicationExitCode.SetParametersFailed, message) { }

        public DisplayCheckerBoardFailedException(string message, Exception innerException)
            : base(ApplicationExitCode.SetParametersFailed, message, innerException) { }
    }
}
