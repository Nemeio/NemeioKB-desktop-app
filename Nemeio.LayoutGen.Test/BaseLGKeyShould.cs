using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Models;
using NUnit.Framework;
using SkiaSharp;

namespace Nemeio.LayoutGen.Test
{
    public class BaseLGKeyShould
    {
        internal LGLayout _fakeLayout;
        internal LGPosition _fakePosition;
        internal LGSize _fakeSize;
        internal Font _fakeFont;

        [SetUp]
        public void SetUp()
        {
            _fakeFont = FontProvider.GetDefaultFont();
            _fakeLayout = new LGLayout(new LGPosition(0, 0), new LGSize(100, 100), _fakeFont, SKColors.Black);
            _fakePosition = new LGPosition(12, 12);
            _fakeSize = new LGSize(34);
        }
    }
}
