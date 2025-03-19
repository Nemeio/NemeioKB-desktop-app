using System;
using System.IO;
using FluentAssertions;
using Moq;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Models;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGImageProviderShould
    {
        LGImageProvider _imageProvider;
        IDocument _mockDocumentService;

        [SetUp]
        public void SetUp()
        {
            _mockDocumentService = Mock.Of<IDocument>();

            Mock.Get(_mockDocumentService)
                .Setup(x => x.GetConfiguratorPath())
                .Returns(string.Empty);

            _imageProvider = new LGImageProvider(_mockDocumentService);
        }

        [Test]
        public void LGImageProvider_01_01_Constructor_WorksOk()
        {
            Assert.DoesNotThrow(() => new LGImageProvider(_mockDocumentService));
            Assert.Throws<ArgumentNullException>(() => new LGImageProvider(null));
        }

        [TestCase("arrowDown.svg")]
        [TestCase("arrowLeft.svg")]
        [TestCase("arrowRight.svg")]
        [TestCase("arrowUp.svg")]
        [TestCase("moon.svg")]
        [TestCase("maj.svg")]
        [TestCase("mute.svg")]
        [TestCase("windows.svg")]
        public void LGImageProvider_01_02_GetImageStream_WithEmbeddedResource_WorksOk(string resourceName)
        {
            IsValidImage(resourceName);
        }

        [TestCase("cmd.svg")]
        [TestCase("menu.svg")]
        [TestCase("moon.svg")]
        [TestCase("windows.svg")]
        public void LGImageProvider_01_03_GetImageStream_WithFilePath_WorksOk(string resourceName)
        {
            var resourcePath = Path.Combine(
                Environment.CurrentDirectory,
                "Resources",
                resourceName
            );

            IsValidImage(resourcePath);
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(@"this\is\a\fake\resource\path\image.png")]
        [TestCase(@"emb://missing_embedded_resource.svg")]
        public void LGImageProvider_01_04_GetImageStream_WithUnknownImage_WorksOk(string resourceName)
        {
            var resourceStream = _imageProvider.GetImageStream(resourceName);

            resourceStream.Should().NotBeNull();
            IsDefaultIcon(resourceStream).Should().BeTrue();
        }

        private void IsValidImage(string resourceName)
        {
            var resourceStream = _imageProvider.GetImageStream(resourceName);

            resourceStream.Should().NotBeNull();
            IsDefaultIcon(resourceStream).Should().BeFalse();
        }

        private bool IsDefaultIcon(Stream imageStream)
        {
            using (var defaultIconStream = Resources.Resources.GetResourceStream($"Images.{LGImageProvider.DefaultIconName}"))
            {
                return StreamEquals(imageStream, defaultIconStream);
            }
        }

        private bool StreamEquals(Stream a, Stream b)
        {
            if (a == b)
            {
                return true;
            }

            if (a == null || b == null)
            {
                throw new ArgumentNullException(a == null ? "a" : "b");
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                var aByte = a.ReadByte();
                var bByte = b.ReadByte();

                if (aByte.CompareTo(bByte) != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
