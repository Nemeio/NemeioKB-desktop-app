using System;
using Nemeio.Core.Services.Batteries;
using Nemeio.Core.Services.TestBench.PressedKeys;

namespace Nemeio.Core.Keyboard.PressedKeys
{
    public interface IGetPressedKeysHolder
    {
        PressedKeysState PressedKeys { get; }

        
    }
}
