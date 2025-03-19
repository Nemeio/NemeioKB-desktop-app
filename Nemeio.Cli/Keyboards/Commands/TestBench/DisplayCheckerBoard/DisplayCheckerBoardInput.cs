using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Parameters;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.TestBench.DisplayCheckerBoard
{
    internal sealed class DisplayCheckerBoardInput
    {
        public byte FirstColor { get; private set; }
        public DisplayCheckerBoardInput(byte firstColor)
        {
            FirstColor = firstColor;
        }
    }
}