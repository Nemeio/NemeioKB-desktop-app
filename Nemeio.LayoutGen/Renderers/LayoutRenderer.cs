using System;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Drawers;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Renderers
{
    internal sealed class LayoutRenderer
    {
        private readonly IFontProvider _fontProvider;
        private readonly IFontSelector _fontSelector;
        private readonly LGTypefaceProvider _typefaceProvider;

        public LayoutRenderer(IFontProvider fontProvider)
        {
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
            _typefaceProvider = new LGTypefaceProvider(_fontProvider);
            _fontSelector = new LGFontSelector(_fontProvider, _typefaceProvider);
        }

        internal SKBitmap RenderImage(LGLayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            var infos = new SKImageInfo((int)layout.Size.Width, (int)layout.Size.Height);
            using (var surface = SKSurface.Create(infos))
            {
                var canvas = surface.Canvas;

                DrawLayout(canvas, layout);

                using (SKImage snap = surface.Snapshot())
                {
                    return SKBitmap.FromImage(snap);
                }
            }
        }

        private void DrawLayout(SKCanvas canvas, LGLayout layout)
        {
            new LGLayoutDrawer(canvas).Draw(layout);

            if (layout.Keys == null)
            {
                return;
            }

            if (layout.Keys.Count == 0)
            {
                return;
            }

            var subKeyDrawer = new LGSubKeyDrawer(canvas, _fontSelector, _typefaceProvider, _fontProvider);

            foreach (var key in layout.Keys)
            {
                if (key.Subkeys != null && key.Subkeys.Count > 0)
                {
                    foreach (var subkey in key.Subkeys)
                    {
                        subKeyDrawer.Draw(subkey);
                    }
                }
            }
        }
    }
}
