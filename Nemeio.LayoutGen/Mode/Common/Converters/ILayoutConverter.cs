using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Images;
using Nemeio.LayoutGen.Models;

namespace Nemeio.LayoutGen.Converters
{
    internal interface ILayoutConverter
    {
        LGLayout Convert(KeyboardModifier modifier, IEnumerable<Key> keys, Font mainFont, bool isDark, IDeviceMap deviceMap, ImageAdjustment imageAdjustment);
    }
}
