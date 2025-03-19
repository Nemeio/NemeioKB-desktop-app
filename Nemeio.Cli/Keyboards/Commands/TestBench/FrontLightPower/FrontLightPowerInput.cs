using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.FrontLightPower
{
    internal sealed class FrontLightPowerInput
    {
        public uint Power { get; private set; }
        public FrontLightPowerInput(uint power)
        {
            Power = power;
        }
    }
}