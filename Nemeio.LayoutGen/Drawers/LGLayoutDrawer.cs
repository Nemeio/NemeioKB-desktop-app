using System;
using Nemeio.LayoutGen.Extensions;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Drawers
{
    internal class LGLayoutDrawer : LGDrawer<LGLayout>
    {
        internal LGLayoutDrawer(SKCanvas canvas)
            : base(canvas) { }

        internal override void Draw(LGLayout layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            _canvas.DrawFilledRectangle(
                layout.Position.X,
                layout.Position.Y,
                layout.Size.Width,
                layout.Size.Height,
                layout.BackgroundColor
            );
        }
    }
}
