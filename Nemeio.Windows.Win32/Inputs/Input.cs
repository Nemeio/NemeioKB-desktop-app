using System.Runtime.InteropServices;

namespace Nemeio.Windows.Win32.Inputs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Input
    {
        public uint Type;

        public MouseKeyboardHardwareInput Data;
    }
}
