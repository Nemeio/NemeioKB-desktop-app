using Nemeio.Core.DataModels.Configurator;
using Nemeio.LayoutGen.Models.Area;

namespace Nemeio.LayoutGen.Models
{
    internal class LGFourfoldKey : LGKey
    {
        internal LGFourfoldKey(LGLayout parent, LGPosition position, LGSize size, Font font) 
            : base(parent, position, size, font) { }

        internal override LGArea GetArea()
        {
            return new LGFourfoldArea();
        }

        internal override float Reduction() => 0.20F;
    }
}
