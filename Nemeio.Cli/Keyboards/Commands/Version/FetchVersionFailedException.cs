using System;
using Nemeio.Cli.Application;
using Nemeio.Cli.Commands.Exceptions;

namespace Nemeio.Cli.Keyboards.Commands.Version
{
    internal sealed class FetchVersionFailedException : CommandFailedException
    {
        public FetchVersionFailedException() 
            : base(ApplicationExitCode.FetchVersionFailed) { }

        public FetchVersionFailedException(string message) 
            : base(ApplicationExitCode.FetchVersionFailed, message) { }

        public FetchVersionFailedException(string message, Exception innerException) 
            : base(ApplicationExitCode.FetchVersionFailed, message, innerException) { }
    }
}
