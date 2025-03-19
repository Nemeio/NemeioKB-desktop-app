using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.CheckComponents
{
    internal sealed class CheckComponentsInput
    {
        public uint ComponentId { get; private set; }
        public CheckComponentsInput(uint componentId)
        {
            ComponentId = componentId;

        }
    }
}
