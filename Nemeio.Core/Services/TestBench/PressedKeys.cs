using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Services.TestBench;
using System;
using System.Collections.Generic;

namespace Nemeio.Core.Services.TestBench.PressedKeys
{
    public class PressedKeysState : TestBenchResultBase
    {
        public List<ushort> Keys { get; private set; }

        public PressedKeysState(KeyboardErrorCode executionResult, List<ushort> keys) :
            base(executionResult)
        {
            Keys = keys;
        }
    }
}
