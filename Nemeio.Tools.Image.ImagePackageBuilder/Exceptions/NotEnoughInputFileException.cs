using Nemeio.Tools.Image.ImagePackageBuilder.Applications;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Exceptions
{
    internal sealed class NotEnoughInputFileException : ImagePackagBuilderException
    {
        public NotEnoughInputFileException()
            : base(ApplicationExitCode.NotEnoughtInputFile) { }

        public NotEnoughInputFileException(string message)
            : base(ApplicationExitCode.NotEnoughtInputFile, message) { }
    }
}
