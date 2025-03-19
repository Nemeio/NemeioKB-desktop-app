using System;
using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    internal class LGLayout : LGComponent
    {
        internal SKColor BackgroundColor { get; }
        internal Font Font { get; }
        internal ISet<LGKey> Keys { get; }

        internal LGLayout(LGPosition position, LGSize size, Font font, SKColor backgroundColor) 
            : base(position, size) 
        {
            BackgroundColor = backgroundColor;
            Keys = new HashSet<LGKey>();
            Font = font ?? throw new ArgumentNullException(nameof(font));
        }
    }
}
