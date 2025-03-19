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
    public class LGImageSubkeyShould
    {
        [Test]
        public void LGImageSubkey_01_01_Constructor_WorksOk()
        {
            const string validFileName = "myFileName.png";

            var fakeFont = FontProvider.GetDefaultFont();
            var fakeKey = new LGSingleKey(
                new LGLayout(new LGPosition(0, 0), new LGSize(1024), fakeFont, SKColors.Black),
                new LGPosition(0, 0),
                new LGSize(12),
                fakeFont
            );
            var mockImageStream = Mock.Of<Stream>();
            var foregroundColor = SKColors.Black;

            Assert.DoesNotThrow(() => new LGImageSubkey(fakeKey, LGSubKeyDispositionArea.None, validFileName, foregroundColor));
            Assert.DoesNotThrow(() => new LGImageSubkey(fakeKey, LGSubKeyDispositionArea.None, validFileName, foregroundColor, mockImageStream));

            Assert.Throws<InvalidOperationException>(() => new LGImageSubkey(fakeKey, LGSubKeyDispositionArea.None, string.Empty, foregroundColor, mockImageStream));
            Assert.Throws<InvalidOperationException>(() => new LGImageSubkey(fakeKey, LGSubKeyDispositionArea.None, " ", foregroundColor, mockImageStream));
            Assert.Throws<InvalidOperationException>(() => new LGImageSubkey(fakeKey, LGSubKeyDispositionArea.None, null, foregroundColor, mockImageStream));

            Assert.Throws<ArgumentNullException>(() => new LGImageSubkey(null, LGSubKeyDispositionArea.None, validFileName, foregroundColor, mockImageStream));
        }
    }
}
