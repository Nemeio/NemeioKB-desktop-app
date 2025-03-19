using System;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Models;
using NUnit.Framework;
using SkiaSharp;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGLayoutShould
    {
        [Test]
        public void LGLayout_01_01_Constructor_WorksOk()
        {
            var fakePosition = new LGPosition(0, 0);
            var fakeSize = new LGSize(0, 0);
            var fakeFont = FontProvider.GetDefaultFont();

            Assert.DoesNotThrow(() => new LGLayout(fakePosition, fakeSize, fakeFont, SKColors.Black));
            Assert.Throws<ArgumentNullException>(() => new LGLayout(null, fakeSize, fakeFont, SKColors.Black));
            Assert.Throws<ArgumentNullException>(() => new LGLayout(fakePosition, null, fakeFont, SKColors.Black));
            Assert.Throws<ArgumentNullException>(() => new LGLayout(fakePosition, fakeSize, null, SKColors.Black));
        }
    }
}
