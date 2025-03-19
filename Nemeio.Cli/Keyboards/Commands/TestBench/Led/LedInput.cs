using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.Led
{
    internal sealed class LedInput
    {
        public uint LedId { get; private set; }
        public uint LedState { get; private set; }

        public LedInput(uint ledId, uint ledState)
        {
            LedId = ledId;
            LedState = ledState;
        }
    }
}
