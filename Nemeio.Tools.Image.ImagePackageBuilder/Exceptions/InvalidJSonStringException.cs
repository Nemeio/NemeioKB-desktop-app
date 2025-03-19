using Nemeio.Tools.Image.ImagePackageBuilder.Applications;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Exceptions
{
    internal sealed class InvalidJSonStringException : ImagePackagBuilderException
    {
        public InvalidJSonStringException()
            : base(ApplicationExitCode.WritePackageFailed) { }

        public InvalidJSonStringException(string message) 
            : base(ApplicationExitCode.WritePackageFailed, message) { }
    }
}
