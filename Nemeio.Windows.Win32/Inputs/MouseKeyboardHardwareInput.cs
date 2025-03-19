using System.Runtime.InteropServices;

namespace Nemeio.Windows.Win32.Inputs
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MouseKeyboardHardwareInput
    {
        [FieldOffset(0)]
        public MouseInput Mouse;

        [FieldOffset(0)]
        public KeyboardInput Keyboard;

        [FieldOffset(0)]
        public HardwareInput Hardware;
    }
}
