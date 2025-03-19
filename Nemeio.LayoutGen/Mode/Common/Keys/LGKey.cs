using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.LayoutGen.Models.Area;

namespace Nemeio.LayoutGen.Models
{
    internal abstract class LGKey : LGSubComponent<LGLayout>
    { 
        internal Font Font { get; }
        internal ISet<LGSubkey> Subkeys { get; private set; }

        internal LGKey(LGLayout parent, LGPosition position, LGSize size, Font font) 
            : base(parent, position, size) 
        {
            Font = font;
            Subkeys = new HashSet<LGSubkey>();
        }

        internal abstract LGArea GetArea();

        internal abstract float Reduction();
    }
}
