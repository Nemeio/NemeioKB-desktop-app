using System;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Drawers
{
    internal class LGSubKeyDrawer : LGDrawer<LGSubkey>
    {
        private readonly IFontSelector _fontSelector;
        private readonly LGTypefaceProvider _typefaceProvider;
        private readonly IFontProvider _fontProvider;

        internal LGSubKeyDrawer(SKCanvas canvas, IFontSelector fontSelector, LGTypefaceProvider typefaceProvider, IFontProvider fontProvider) 
            : base(canvas) 
        {
            _fontSelector = fontSelector ?? throw new ArgumentNullException(nameof(fontSelector));
            _typefaceProvider = typefaceProvider ?? throw new ArgumentNullException(nameof(fontSelector));
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
        }

        internal override void Draw(LGSubkey subkey)
        {
            if (subkey == null)
            {
                throw new ArgumentNullException(nameof(subkey));
            }

            if (subkey is LGImageSubkey imageSubKey)
            {
                new LGImageSubKeyDrawer(_canvas).Draw(imageSubKey);
            }
            else if (subkey is LGTextSubkey textSubKey)
            {
                new LGTextSubKeyDrawer(_canvas, _fontSelector, _typefaceProvider, _fontProvider).Draw(textSubKey);
            }
            else
            {
                throw new InvalidOperationException("Unsupported subkey type");
            }
        }
    }
}