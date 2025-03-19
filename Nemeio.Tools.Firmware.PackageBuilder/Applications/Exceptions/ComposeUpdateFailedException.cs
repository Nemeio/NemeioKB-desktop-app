using System;
using System.Runtime.Serialization;
using Nemeio.Tools.Core;

namespace Nemeio.Tools.Firmware.PackageBuilder.Applications.Exceptions
{
    internal class ComposeUpdateFailedException : ApplicationException<ApplicationExitCode>
    {
        public ComposeUpdateFailedException() 
            : base(ApplicationExitCode.ComposeUpdateFailed)
        {
        }

        public ComposeUpdateFailedException(string message) 
            : base(ApplicationExitCode.ComposeUpdateFailed, message)
        {
        }

        public ComposeUpdateFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        public ComposeUpdateFailedException(string message, Exception innerException) 
            : base(ApplicationExitCode.ComposeUpdateFailed, message, innerException)
        {
        }
    }
}
