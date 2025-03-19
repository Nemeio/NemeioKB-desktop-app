using FluentAssertions;
using Nemeio.Tools.Image.ImagePackageBuilder.Applications;
using Nemeio.Tools.Image.ImagePackageBuilder.Exceptions;
using NUnit.Framework;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Tests.Exceptions
{
    [TestFixture]
    public sealed class WritePackageFailedExceptionShould
    {
        [Test]
        public void WritePackageFailedException_ExitCode_Ok()
        {
            var exception = new WritePackageFailedException();

            exception.ExitCode.Should().Be(ApplicationExitCode.WritePackageFailed);
        }
    }
}
