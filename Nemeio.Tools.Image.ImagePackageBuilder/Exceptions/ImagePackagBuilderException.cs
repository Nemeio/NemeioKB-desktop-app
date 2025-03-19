using System;
using System.Runtime.Serialization;
using Nemeio.Tools.Core;
using Nemeio.Tools.Image.ImagePackageBuilder.Applications;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Exceptions
{
    internal abstract class ImagePackagBuilderException : ApplicationException<ApplicationExitCode>
    {
        public ImagePackagBuilderException(ApplicationExitCode exitCode)
            : base(exitCode) { }

        public ImagePackagBuilderException(ApplicationExitCode exitCode, string message)
            : base(exitCode, message) { }

        public ImagePackagBuilderException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public ImagePackagBuilderException(ApplicationExitCode exitCode, string message, Exception innerException)
            : base(exitCode, message, innerException) { }
    }
}
