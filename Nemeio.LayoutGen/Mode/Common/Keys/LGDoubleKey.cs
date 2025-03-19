using Nemeio.Core.DataModels.Configurator;
using Nemeio.LayoutGen.Models.Area;

namespace Nemeio.LayoutGen.Models
{
    internal class LGDoubleKey : LGKey
    {
        internal LGDoubleKey(LGLayout parent, LGPosition position, LGSize size, Font font) 
            : base(parent, position, size, font) { }

        internal override LGArea GetArea()
        {
            return new LGDoubleArea();
        }

        internal override float Reduction() => 0.10F;
    }
}
