using System;
using System.IO;
using Moq;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Models;
using NUnit.Framework;
using SkiaSharp;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class LGTextSubkeyShould
    {
        [Test]
        public void LGTextSubkey_01_01_Constructor_WorksOk()
        {
            const string validText = "A";

            var fakeFont = FontProvider.GetDefaultFont();
            var fakeKey = new LGSingleKey(
                new LGLayout(new LGPosition(0, 0), new LGSize(1024), fakeFont, SKColors.Black),
                new LGPosition(0, 0),
                new LGSize(12),
                fakeFont
            );
            var mockImageStream = Mock.Of<Stream>();
            var foregroundColor = SKColors.Black;

            Assert.DoesNotThrow(() => new LGTextSubkey(fakeKey, LGSubKeyDispositionArea.None, validText, foregroundColor));
            Assert.Throws<ArgumentNullException>(() => new LGTextSubkey(fakeKey, LGSubKeyDispositionArea.None, null, foregroundColor));
            Assert.Throws<ArgumentNullException>(() => new LGTextSubkey(null, LGSubKeyDispositionArea.None, validText, foregroundColor));
        }
    }
}
