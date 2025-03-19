using System;
using FluentAssertions;
using Nemeio.LayoutGen.Exceptions;
using Nemeio.LayoutGen.Factory;
using Nemeio.LayoutGen.Models.Loader;
using NUnit.Framework;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGImageLoaderFactoryShould
    {
        LGImageLoaderFactory _loaderFactory;

        [SetUp]
        public void SetUp()
        {
            _loaderFactory = new LGImageLoaderFactory();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void LGImageLoaderFactory_01_01_CreateImageLoader_WithInvalidArgument_ThrowsInvalidOperationException(string path)
        {
            Assert.Throws<InvalidOperationException>(() => 
            {
                _loaderFactory.CreateImageLoader(path);
            });
        }

        [TestCase("myFile.png")]
        [TestCase("myFile.svg")]
        public void LGImageLoaderFactory_01_02_CreateImageLoader_WithValidExtension_NotThrowsException(string path)
        {
            Assert.DoesNotThrow(() => 
            {
                _loaderFactory.CreateImageLoader(path);
            });
        }

        [TestCase("myFile.xml")]
        [TestCase("myFile.gif")]
        [TestCase("myImage.jpg")]
        [TestCase("myImage.ico")]
        public void LGImageLoaderFactory_01_03_CreateImageLoader_WithUnknownExtension_ThrowsImageFormatNotSupportedException(string path)
        {
            Assert.Throws<ImageFormatNotSupportedException>(() =>
            {
                _loaderFactory.CreateImageLoader(path);
            });
        }

        [Test]
        public void LGImageLoaderFactory_01_04_CreateImageLoader_CreatePngLoader()
        {
            const string filePath = "my/path/myFile.png";

            var loader = _loaderFactory.CreateImageLoader(filePath);

            loader.GetType().Should().Be(typeof(LGPngLoader));
        }

        [Test]
        public void LGImageLoaderFactory_01_05_CreateImageLoader_CreateSvgLoader()
        {
            const string filePath = "my/path/myFile.svg";

            var loader = _loaderFactory.CreateImageLoader(filePath);

            loader.GetType().Should().Be(typeof(LGSvgLoader));
        }
    }
}
