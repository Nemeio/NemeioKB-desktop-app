using System.IO;
using Moq;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Image.ImagePackageBuilder.Files;
using NUnit.Framework;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Tests.Files
{
    [TestFixture]
    public sealed class JpgInputFileShould
    {
        private IFileSystem _mockFileSystem;

        [SetUp]
        public void SetUp()
        {
            _mockFileSystem = Mock.Of<IFileSystem>();

            Mock.Get(_mockFileSystem)
                .Setup(x => x.GetFileExtension(It.IsAny<string>()))
                .Returns((string input) => input);

            Mock.Get(_mockFileSystem)
                .Setup(x => x.FileExists(It.IsAny<string>()))
                .Returns(true);
        }

        [TestCase(".bmp")]
        public void BmpInputFile_Constructor_Ok(string extension)
        {
            Assert.DoesNotThrow(() => new BmpInputFile(_mockFileSystem, extension));
        }

        [TestCase(".png")]
        [TestCase(".jpg")]
        [TestCase(".pdf")]
        [TestCase(".gif")]
        [TestCase(".ico")]
        public void JpgInputFile_Constructor_WhenExtensionisNotValid_Throws(string extension)
        {
            Assert.Throws<InvalidDataException>(() => new BmpInputFile(_mockFileSystem, extension));
        }
    }
}
