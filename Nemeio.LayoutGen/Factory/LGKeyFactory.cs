using System;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.LayoutGen.Models;

namespace Nemeio.LayoutGen.Factory
{
    internal class LGKeyFactory
    {
        public LGKey CreateKey(LGLayout layout, Key key, IDeviceMap deviceMap)
        {
            var keyScanCode = deviceMap.Buttons.ElementAt(key.Index);
            var position = deviceMap.PositionOfButton(keyScanCode);
            var size = deviceMap.SizeOfButton(keyScanCode);

            switch (key.Disposition)
            {
                case KeyDisposition.Single:
                    return new LGSingleKey(layout, position, size, key.Font);
                case KeyDisposition.Double:
                    return new LGDoubleKey(layout, position, size, key.Font);
                case KeyDisposition.Full:
                    return new LGFourfoldKey(layout, position, size, key.Font);
                case KeyDisposition.TShape:
                    return new LGThreeKey(layout, position, size, key.Font);
                default:
                    throw new InvalidOperationException("Unknown disposition");
            }
        }
    }
}
