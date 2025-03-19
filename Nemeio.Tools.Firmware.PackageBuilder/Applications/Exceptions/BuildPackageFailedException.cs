using System;
using System.Runtime.Serialization;

namespace Nemeio.Tools.Firmware.PackageBuilder.Applications.Exceptions
{
    internal class BuildPackageFailedException : Nemeio.Tools.Core.ApplicationException<ApplicationExitCode>
    {
        public BuildPackageFailedException()
            : base(ApplicationExitCode.BuildPackageFailed)
        {
        }

        public BuildPackageFailedException(string message) : base(ApplicationExitCode.BuildPackageFailed, message)
        {
        }

        public BuildPackageFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public BuildPackageFailedException(string message, Exception innerException) : base(ApplicationExitCode.BuildPackageFailed, message, innerException)
        {
        }
    }
}
