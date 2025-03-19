using System;
using System.Linq;
using FluentAssertions;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Drawers;
using Nemeio.LayoutGen.Models;
using NUnit.Framework;
using SkiaSharp;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGLayoutDrawerShould
    {
        [Test]
        public void LGLayoutDrawer_01_01_Constructor_WorksOk()
        {
            SKImageInfo infos = new SKImageInfo(128, 128);
            using (var surface = SKSurface.Create(infos))
            {
                Assert.DoesNotThrow(() => new LGLayoutDrawer(surface.Canvas));
            }

            Assert.Throws<ArgumentNullException>(() => new LGLayoutDrawer(null));
        }

        [Test]
        public void LGLayoutDrawer_01_02_Draw_WithNullParameter_ThrowsArgumentNullException()
        {
            SKImageInfo infos = new SKImageInfo(128, 128);
            using (var surface = SKSurface.Create(infos))
            {
                var layoutDrawer = new LGLayoutDrawer(surface.Canvas);

                Assert.Throws<ArgumentNullException>(() => layoutDrawer.Draw(null));
            }
        }

        [Test]
        public void LGLayoutDrawer_01_03_Draw_WorksOk()
        {
            var lgLayout = new LGLayout(
                new LGPosition(0, 0),
                new LGSize(128),
                FontProvider.GetDefaultFont(),
                SKColors.Black
            );

            var infos = new SKImageInfo(128, 128);
            using (var surface = SKSurface.Create(infos))
            {
                var layoutDrawer = new LGLayoutDrawer(surface.Canvas);
                layoutDrawer.Draw(lgLayout);

                using (var snap = surface.Snapshot())
                using (var bitmap = SKBitmap.FromImage(snap))
                {
                    var sample = FileHelper.ByteArrayFromResources("lgLayoutDrawer.bytes");

                    bitmap.Bytes.SequenceEqual(sample).Should().BeTrue();
                }
            }
        }
    }
}
