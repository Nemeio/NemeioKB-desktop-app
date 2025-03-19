using System;
using System.Collections.Generic;
using Moq;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Images.Jpeg;
using Nemeio.Core.Images.Jpeg.Builder;
using Nemeio.Tools.Core.Files;
using Nemeio.Tools.Image.ImagePackageBuilder.Applications;
using Nemeio.Tools.Image.ImagePackageBuilder.Exceptions;
using Nemeio.Tools.Image.ImagePackageBuilder.Packages.Writer;
using NUnit.Framework;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Tests.Applications
{
    [TestFixture]
    public sealed class ApplicationShould
    {
        [Test]
        public void Application_Constructor_Ok()
        {
            var mockFileSystem = Mock.Of<IFileSystem>();
            var mockImagePackageWriter = Mock.Of<IImagePackageWriter>();
            var mockJpegImagePackageBuilder = Mock.Of<IJpegImagePackageBuilder>();

            Assert.Throws<ArgumentNullException>(() => new Application(null, mockImagePackageWriter, mockJpegImagePackageBuilder));
            Assert.Throws<ArgumentNullException>(() => new Application(mockFileSystem, null, mockJpegImagePackageBuilder));
            Assert.Throws<ArgumentNullException>(() => new Application(mockFileSystem, mockImagePackageWriter, null));
            Assert.DoesNotThrow(() => new Application(mockFileSystem, mockImagePackageWriter, mockJpegImagePackageBuilder));
        }

        [Test]
        public void Application_RunAsync_WhenImagesPathOnSettings_AreNull_Throws()
        {
            var mockFileSystem = Mock.Of<IFileSystem>();
            var mockImagePackageWriter = Mock.Of<IImagePackageWriter>();
            var mockJpegImagePackageBuilder = Mock.Of<IJpegImagePackageBuilder>();

            var application = new Application(mockFileSystem, mockImagePackageWriter, mockJpegImagePackageBuilder);
            application.Settings = new ApplicationStartupSettings()
            {
                ImagesPath = null,
                OutputPath = "this\\is\\my\fake\\path",
                CompressionType = "none"
            };

            Assert.ThrowsAsync<NotEnoughInputFileException>(() => application.RunAsync());
        }

        [Test]
        public void Application_RunAsync_WhenImagesPathOnSettings_AreEmpty_Throws()
        {
            var mockFileSystem = Mock.Of<IFileSystem>();
            var mockImagePackageWriter = Mock.Of<IImagePackageWriter>();
            var mockJpegImagePackageBuilder = Mock.Of<IJpegImagePackageBuilder>();

            var application = new Application(mockFileSystem, mockImagePackageWriter, mockJpegImagePackageBuilder);
            application.Settings = new ApplicationStartupSettings()
            {
                ImagesPath = new List<string>(),
                OutputPath = "this\\is\\my\fake\\path",
                CompressionType = "none"
            };

            Assert.ThrowsAsync<NotEnoughInputFileException>(() => application.RunAsync());
        }

        [Test]
        public void Application_RunAsync_WhenWritePackageOnFileSystem_Fail_Throws()
        {
            var mockFileSystem = Mock.Of<IFileSystem>();
            Mock.Get(mockFileSystem)
                .Setup(x => x.ReadByteArrayAsync(It.IsAny<string>()))
                .ReturnsAsync(new byte[1] { 0 });
            Mock.Get(mockFileSystem)
                .Setup(x => x.FileExists(It.IsAny<string>()))
                .Returns(true);
            Mock.Get(mockFileSystem)
                .Setup(x => x.GetFileExtension(It.IsAny<string>()))
                .Returns(".bmp");

            var mockImagePackageWriter = Mock.Of<IImagePackageWriter>();
            Mock.Get(mockImagePackageWriter)
                .Setup(x => x.WriteOnDiskAsync(It.IsAny<JpegImagePackage>(), It.IsAny<string>(), It.IsAny<LayoutFileDto>()))
                .Throws(new Exception("This is a fake exception"));

            var mockJpegImagePackageBuilder = Mock.Of<IJpegImagePackageBuilder>();

            var application = new Application(mockFileSystem, mockImagePackageWriter, mockJpegImagePackageBuilder);
            application.Settings = new ApplicationStartupSettings()
            {
                ImagesPath = new List<string>() { "this\\is\\my\fake\\path\\to\\image.bmp" },
                OutputPath = "this\\is\\my\fake\\path",
                CompressionType = "none"
            };

            Assert.ThrowsAsync<System.IO.FileNotFoundException>(() => application.RunAsync());
        }
    }
}
