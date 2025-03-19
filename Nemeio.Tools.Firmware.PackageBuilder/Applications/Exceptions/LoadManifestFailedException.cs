using System.Runtime.Serialization;
using Nemeio.Tools.Core;

namespace Nemeio.Tools.Firmware.PackageBuilder.Applications.Exceptions
{
    internal class LoadManifestFailedException : ApplicationException<ApplicationExitCode>
    {
        public LoadManifestFailedException() 
            : base(ApplicationExitCode.LoadManifestFailed)
        {
        }

        public LoadManifestFailedException(string message) 
            : base(ApplicationExitCode.LoadManifestFailed, message)
        {
        }

        public LoadManifestFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        public LoadManifestFailedException(string message, System.Exception innerException) 
            : base(ApplicationExitCode.LoadManifestFailed, message, innerException)
        {
        }
    }
}
