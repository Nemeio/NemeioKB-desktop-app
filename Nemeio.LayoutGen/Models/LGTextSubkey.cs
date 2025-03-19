using System;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    internal class LGTextSubkey : LGSubkey
    {
        internal string Text { get; private set; }
        internal SKColor TextColor { get; private set; }
        internal bool ForceBold { get; private set; }

        internal LGTextSubkey(LGKey parent, LGSubKeyDispositionArea dispositionArea, string text, SKColor textColor, bool forceBold = false)
            : base(parent, dispositionArea)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            TextColor = textColor;
            ForceBold = forceBold;
        }
    }
}
