using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Images.Jpeg;
using Nemeio.Tools.Core.Files;
using Nemeio.Tools.Image.ImagePackageBuilder.Packages.Writer;
using NUnit.Framework;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Tests.Package
{
    [TestFixture]
    public sealed class ImagePackageWriterShould
    {
        [Test]
        public void ImagePackageWriter_Constructor_Ok()
        {
            Assert.DoesNotThrow(() => new ImagePackageWriter(Mock.Of<IFileSystem>()));
            Assert.Throws<ArgumentNullException>(() => new ImagePackageWriter(null));
        }

        [Test]
        public void ImagePackageWriter_WriteOnDiskAsync_WhenPackageIsNull_Throws()
        {
            var mockFileSystem = Mock.Of<IFileSystem>();
            var writer = new ImagePackageWriter(mockFileSystem);

            Assert.ThrowsAsync<ArgumentNullException>(() => writer.WriteOnDiskAsync(null, "this\\is\\a\fake\\path", It.IsAny<LayoutFileDto>()));
        }

        [Test]
        public void ImagePackageWriter_WriteOnDiskAsync_WhenOutputFilePath_IsNotNullOrEmpty_ButDirectoryDoesntExists_Throws()
        {
            const string outputFilePath = "this\\is\\my\\fake\\current\\directory";

            var mockFileSystem = Mock.Of<IFileSystem>();
            Mock.Get(mockFileSystem)
                .Setup(x => x.GetDirectoryName(It.IsAny<string>()))
                .Returns("MyDirectoryName");
            Mock.Get(mockFileSystem)
                .Setup(x => x.DirectoryExists(It.IsAny<string>()))
                .Returns(false);

            var package = new JpegImagePackage();
            package.Header = new JpegImagePackageHeader();
            package.Images = new List<JpegImage>();
            var layoutFileDto = new LayoutFileDto();
            var writer = new ImagePackageWriter(mockFileSystem);

            Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteOnDiskAsync(package, outputFilePath, layoutFileDto));
        }

        [Test]
        public void ImagePackageWriter_WriteOnDiskAsync_WhenOutputFilePath_IsNotNullOrEmpty_ButDoesntHaveFilename_Throws()
        {
            const string outputFilePath = "this\\is\\my\\fake\\current\\directory";

            var mockFileSystem = Mock.Of<IFileSystem>();
            Mock.Get(mockFileSystem)
                .Setup(x => x.GetDirectoryName(It.IsAny<string>()))
                .Returns("MyDirectoryName");
            Mock.Get(mockFileSystem)
                .Setup(x => x.DirectoryExists(It.IsAny<string>()))
                .Returns(true);
            Mock.Get(mockFileSystem)
                .Setup(x => x.GetFileName(It.IsAny<string>()))
                .Returns(string.Empty);

            var package = new JpegImagePackage();
            package.Header = new JpegImagePackageHeader();
            package.Images = new List<JpegImage>();

            var writer = new ImagePackageWriter(mockFileSystem);
            var layoutFileDto = new LayoutFileDto();

            Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteOnDiskAsync(package, outputFilePath, layoutFileDto));
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task ImagePackageWriter_WriteOnDiskAsync_WhenOutputFilePath_IsNullOrEmpty_ButDoesntHaveFilename_Throws(string outputFilePath)
        {
            const string fakeCurrentDirectory = "this\\is\\my\\fake\\current\\directory";

            var writeAsyncCalled = false;
            var writeAsyncAtPath = string.Empty;

            var mockFileSystem = Mock.Of<IFileSystem>();



            Mock.Get(mockFileSystem)
                .Setup(x => x.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback((string input, byte[] data) =>
                {
                    writeAsyncCalled = true;
                    writeAsyncAtPath = input;
                })
                .Returns(Task.Delay(0));

            Mock.Get(mockFileSystem)
                .Setup(x => x.GetCurrentDirectory())
                .Returns(fakeCurrentDirectory);

            var package = new JpegImagePackage();
            package.Header = new JpegImagePackageHeader();
            package.Images = new List<JpegImage>();

            var layoutFileDto = new LayoutFileDto();
            var writer = new ImagePackageWriter(mockFileSystem);


            await writer.WriteOnDiskAsync(package, outputFilePath, layoutFileDto);

            writeAsyncCalled.Should().BeTrue();
            writeAsyncAtPath.Should().NotBeNullOrEmpty();
            writeAsyncAtPath.Should().Be($"{fakeCurrentDirectory}\\{ImagePackageWriter.DefaultOutputFileName}");
        }
    }
}
