using System;
using Nemeio.Core.Systems.Hid;
using Nemeio.Mac.Services;
using Nemeio.Platform.Mac;

namespace Nemeio.Platform.Hid.Mac.Keyboards
{
    public class MacSystemModifierDelegate : ISystemModifierDelegate
    {
        public bool IsModifierKey(string key) => MacKeyboardConstants.IsModifierKey(key);
    }
}
