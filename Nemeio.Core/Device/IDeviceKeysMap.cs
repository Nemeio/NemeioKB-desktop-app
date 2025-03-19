using System.Collections.Generic;

namespace Nemeio.Core.Device
{
    public interface IDeviceKeyMap
    {
        IList<uint> Buttons { get; }
        bool IsModifierKey(int keyIndex);
        bool IsFirstLineKey(int keyIndex);
    }
}
