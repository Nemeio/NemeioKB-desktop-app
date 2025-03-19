using Nemeio.Core.DataModels.Configurator;
using Nemeio.LayoutGen.Models.Area;

namespace Nemeio.LayoutGen.Models
{
    internal class LGSingleKey : LGKey
    {
        internal LGSingleKey(LGLayout parent, LGPosition position, LGSize size, Font font) 
            : base(parent, position, size, font) { }

        internal override LGArea GetArea()
        {
            return new LGSingleArea();
        }

        internal override float Reduction() => 0.0f;
    }
}
