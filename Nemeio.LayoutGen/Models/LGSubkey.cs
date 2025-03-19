using System;

namespace Nemeio.LayoutGen.Models
{
    internal abstract class LGSubkey
    {
        internal LGKey ParentKey { get; private set; }
        internal LGSubKeyDispositionArea Position { get; }

        internal LGSubkey(LGKey parent, LGSubKeyDispositionArea position)
        {
            ParentKey = parent ?? throw new ArgumentNullException(nameof(parent));
            Position = position;
        }
    }
}
