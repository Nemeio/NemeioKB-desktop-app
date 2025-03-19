using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.SetAdvertising
{
    internal sealed class SetAdvertisingInput
    {
        public uint Enable { get; private set; }
        public SetAdvertisingInput(uint enable)
        {
            Enable = enable;
        }
    }
}