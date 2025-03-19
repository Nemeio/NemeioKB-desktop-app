using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Errors;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Drawers;
using Nemeio.LayoutGen.Models;
using Nemeio.Models.Fonts;
using NUnit.Framework;
using SkiaSharp;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGSubKeyDrawerShould
    {
        private IFontSelector _mockFontSelector;
        private LGTypefaceProvider _mockTypefaceProvider;
        private IFontProvider _fontProvider;

        [SetUp]
        public void SetUp()
        {
            _mockFontSelector = Mock.Of<IFontSelector>();

            Mock.Get(_mockFontSelector)
                .Setup(x => x.FallbackFontIfNeeded(It.IsAny<Font>(), It.IsAny<string>()))
                .Returns(new Font(NemeioConstants.Noto, FontSize.Medium, false, false));

            _fontProvider = new FontProvider(new LoggerFactory(), Mock.Of<IErrorManager>());
            _mockTypefaceProvider = new LGTypefaceProvider(_fontProvider);
        }

        [Test]
        public void LGSubKeyDrawer_01_01_Constructor_WorksOk()
        {
            SKImageInfo infos = new SKImageInfo(128, 128);
            using (var surface = SKSurface.Create(infos))
            {
                Assert.DoesNotThrow(() => new LGSubKeyDrawer(surface.Canvas, _mockFontSelector, _mockTypefaceProvider, _fontProvider));
            }

            Assert.Throws<ArgumentNullException>(() => new LGSubKeyDrawer(null, _mockFontSelector, _mockTypefaceProvider, _fontProvider));

            using (var surface = SKSurface.Create(infos))
            {
                Assert.Throws<ArgumentNullException>(() => new LGSubKeyDrawer(surface.Canvas, null, _mockTypefaceProvider, _fontProvider));
                Assert.Throws<ArgumentNullException>(() => new LGSubKeyDrawer(surface.Canvas, _mockFontSelector, null, _fontProvider));
                Assert.Throws<ArgumentNullException>(() => new LGSubKeyDrawer(surface.Canvas, _mockFontSelector, _mockTypefaceProvider, null));
            }
        }

        [Test]
        public void LGSubKeyDrawer_01_02_Draw_WithNullParameter_ThrowsArgumentNullException()
        {
            SKImageInfo infos = new SKImageInfo(128, 128);
            using (var surface = SKSurface.Create(infos))
            {
                var subKeyDrawer = new LGSubKeyDrawer(surface.Canvas, _mockFontSelector, _mockTypefaceProvider, _fontProvider);

                Assert.Throws<ArgumentNullException>(() => subKeyDrawer.Draw(null));
            }
        }

        [Test]
        public void LGSubKeyDrawer_01_03_TextSubKey_Draw_WorksOk()
        {
            var lgSubKey = new LGTextSubkey(
                GetParentKey(),
                LGSubKeyDispositionArea.None, 
                "A",
                SKColors.Black
            );

            var infos = new SKImageInfo(128, 128);
            using (var surface = SKSurface.Create(infos))
            {
                var subKeyDrawer = new LGSubKeyDrawer(surface.Canvas, _mockFontSelector, _mockTypefaceProvider, _fontProvider);
                subKeyDrawer.Draw(lgSubKey);

                using (var snap = surface.Snapshot())
                using (var bitmap = SKBitmap.FromImage(snap))
                {
                    var sample = FileHelper.ByteArrayFromResources("lgTextSubkeyDrawer.bytes");

                    bitmap.Bytes.SequenceEqual(sample).Should().BeTrue();
                }
            }
        }

        [Test]
        public void LGSubKeyDrawer_01_04_ImageSubKey_Draw_WorksOk()
        {
            var lgSubKey = new LGImageSubkey(
                GetParentKey(),
                LGSubKeyDispositionArea.None,
                "cmd.svg",
                SKColors.White,
                FileHelper.StreamFromResources("cmd.svg")
            );

            var infos = new SKImageInfo(128, 128);
            using (var surface = SKSurface.Create(infos))
            {
                var subKeyDrawer = new LGSubKeyDrawer(surface.Canvas, _mockFontSelector, _mockTypefaceProvider, _fontProvider);
                subKeyDrawer.Draw(lgSubKey);

                using (var snap = surface.Snapshot())
                using (var bitmap = SKBitmap.FromImage(snap))
                {
                    var sample = FileHelper.ByteArrayFromResources("lgImageSubkeyDrawer.bytes");

                    bitmap.Bytes.SequenceEqual(sample).Should().BeTrue();
                }
            }
        }

        private LGKey GetParentKey()
        {
            var lgLayout = new LGLayout(
                new LGPosition(0, 0),
                new LGSize(128),
                FontProvider.GetDefaultFont(),
                SKColors.Black
            );

            return new LGSingleKey(
                lgLayout,
                new LGPosition(0, 0),
                new LGSize(83, 83),
                null
            );
        }
    }
}
