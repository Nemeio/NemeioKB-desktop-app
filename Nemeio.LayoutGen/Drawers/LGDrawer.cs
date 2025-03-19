using System;
using SkiaSharp;

namespace Nemeio.LayoutGen.Drawers
{
    internal abstract class LGDrawer<T>
    {
        protected readonly SKCanvas _canvas;

        internal LGDrawer(SKCanvas canvas)
        {
            _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }

        internal abstract void Draw(T element);
    }
}
