using Nemeio.Tools.Image.ImagePackageBuilder.Applications;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Exceptions
{
    internal sealed class WritePackageFailedException : ImagePackagBuilderException
    {
        public WritePackageFailedException()
            : base(ApplicationExitCode.WritePackageFailed) { }

        public WritePackageFailedException(string message) 
            : base(ApplicationExitCode.WritePackageFailed, message) { }
    }
}
