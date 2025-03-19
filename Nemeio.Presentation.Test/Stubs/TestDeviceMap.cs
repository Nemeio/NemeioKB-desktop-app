using System.Collections.Generic;
using Nemeio.LayoutGen.Models;

namespace Nemeio.Presentation.Test.Stubs
{
    public class TestDeviceMap : IDeviceMap
    {
        public LGSize DeviceSize => new LGSize(300);

        public IList<uint> Buttons => new List<uint>()
        {
            0x1,
            0x2,
            0x3
        };

        public IList<Button> RequiredButtons => new List<Button>()
        {
            new Button(0x2, "Ctrl"),
            new Button(0x3, "F1")
        };

        public IList<Button> FnButtons => new List<Button>()
        {
            new Button(0x3, "F1")
        };

        public LGPosition PositionOfButton(uint scanCode) => LGPosition.Zero;

        public LGSize SizeOfButton(uint scanCode) => LGSize.Zero;

        public bool IsFirstLineKey(int keyIndex) => false;

        public bool IsModifierKey(int keyIndex) => false;
    }
}
