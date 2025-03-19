using System;
using System.IO;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    internal class LGImageSubkey : LGSubkey
    {
        internal string Image { get; private set; }
        internal Stream ImageStream { get; private set; }
        internal SKColor ImageColor { get; private set; }
        internal bool ForceBold { get; private set; }

        internal LGImageSubkey(LGKey parent, LGSubKeyDispositionArea dispostionArea, string image, SKColor color, Stream imageStream = null, bool forceBold = false) 
            : base(parent, dispostionArea)
        {
            if (string.IsNullOrWhiteSpace(image))
            {
                throw new InvalidOperationException(nameof(image));
            }

            Image = image;
            ImageStream = imageStream;
            ImageColor = color;
            ForceBold = forceBold;
        }
    }
}
