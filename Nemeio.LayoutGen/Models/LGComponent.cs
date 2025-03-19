using System;

namespace Nemeio.LayoutGen.Models
{
    internal abstract class LGComponent
    {
        internal LGPosition Position { get; private set; }

        internal LGSize Size { get; private set; }

        internal LGComponent(LGPosition position, LGSize size)
        {
            Position = position ?? throw new ArgumentNullException(nameof(position));
            Size = size ?? throw new ArgumentNullException(nameof(position));
        }
    }
}
