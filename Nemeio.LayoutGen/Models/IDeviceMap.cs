using System.Collections.Generic;
using Nemeio.Core.Device;

namespace Nemeio.LayoutGen.Models
{
    public interface IDeviceMap : IDeviceKeyMap
    {
        LGSize DeviceSize { get; }
        IList<Button> RequiredButtons { get; }
        IList<Button> FnButtons { get; }
        LGPosition PositionOfButton(uint scanCode);
        LGSize SizeOfButton(uint scanCode);
    }
}
