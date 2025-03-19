using System;
using System.IO;
using FluentAssertions;
using Moq;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Settings.Parser;
using NUnit.Framework;

namespace Nemeio.Core.Test.Settings
{
    [TestFixture]
    internal class XmlSettingsParserShould
    {
        [Test]
        public void XmlSettingsParser_Parse_WhenFileDoesNotExists_Throws()
        {
            var mockFileSystem = Mock.Of<IFileSystem>();
            Mock.Get(mockFileSystem)
                .Setup(x => x.FileExists(It.IsAny<string>()))
                .Returns(false);

            var myFilePath = @"C:\Users\username\this\is\my\file\path.xml";

            var parser = new XmlSettingsParser(mockFileSystem);

            Assert.Throws<ArgumentException>(() => parser.Parse(myFilePath));
        }

        [Test]
        public void XmlSettingsParser_Parse_WhenFileContentFormatIsInvalid_Throws()
        {
            var settingsContent = "{ 'myKey': 'this_is_my_value' }";
            var tmpFilePath = Path.Combine(Path.GetTempPath(), "fakeSettings.xml");

            using (var tmpFile = new TemporaryFile(tmpFilePath))
            {
                tmpFile.WriteAndClose(settingsContent);

                var fileSystem = new FileSystem.FileSystem();
                var parser = new XmlSettingsParser(fileSystem);

                Assert.Throws<InvalidOperationException>(() => parser.Parse(tmpFilePath));
            }
        }

        [Test]
        public void XmlSettingsParser_Parse_Ok()
        {
            var settingsContent = "<settings><swaggerEnable>true</swaggerEnable><apiPort>1234</apiPort><environment>testing</environment><jpegCompressionPercent>80</jpegCompressionPercent></settings>";
            var tmpFilePath = Path.Combine(Path.GetTempPath(), "fakeSettings.xml");

            using (var tmpFile = new TemporaryFile(tmpFilePath))
            {
                tmpFile.WriteAndClose(settingsContent);

                var fileSystem = new FileSystem.FileSystem();
                var parser = new XmlSettingsParser(fileSystem);

                var result = parser.Parse(tmpFilePath);

                result.Should().NotBeNull();
                result.SwaggerEnable.Should().NotBeNull();
                result.SwaggerEnable.Should().BeTrue();
                result.ApiPort.Should().NotBeNull();
                result.ApiPort.Should().Be("1234");
                result.Environment.Should().NotBeNull();
                result.Environment.Should().Be("testing");
                result.JpegCompressionPercent.Should().NotBeNull();
                result.JpegCompressionPercent.Should().Be(80);
            }
        }
        [Test]
        public void XmlSettingsParser_ApiAuto_Parse_Ok()
        {
            var settingsContent = "<settings><swaggerEnable>true</swaggerEnable><apiPort>auto</apiPort><environment>testing</environment><jpegCompressionPercent>80</jpegCompressionPercent></settings>";
            var tmpFilePath = Path.Combine(Path.GetTempPath(), "fakeSettings.xml");

            using (var tmpFile = new TemporaryFile(tmpFilePath))
            {
                tmpFile.WriteAndClose(settingsContent);

                var fileSystem = new FileSystem.FileSystem();
                var parser = new XmlSettingsParser(fileSystem);

                var result = parser.Parse(tmpFilePath);

                result.Should().NotBeNull();
                result.SwaggerEnable.Should().NotBeNull();
                result.SwaggerEnable.Should().BeTrue();
                result.ApiPort.Should().NotBeNull();
                result.ApiPort.Should().Be("auto");
                result.Environment.Should().NotBeNull();
                result.Environment.Should().Be("testing");
                result.JpegCompressionPercent.Should().NotBeNull();
                result.JpegCompressionPercent.Should().Be(80);
            }
        }
    }
}
