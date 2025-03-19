using NUnit.Framework;
using Nemeio.Tools.Image.ImagePackageBuilder.Exceptions;
using FluentAssertions;
using Nemeio.Tools.Image.ImagePackageBuilder.Applications;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Tests.Exceptions
{
    [TestFixture]
    public sealed class NotEnoughtInputFileExceptionShould
    {
        [Test]
        public void NotEnoughtInputFileException_ExitCode_Ok()
        {
            var exception = new NotEnoughInputFileException();
            
            exception.ExitCode.Should().Be(ApplicationExitCode.NotEnoughtInputFile);
        }
    }
}
