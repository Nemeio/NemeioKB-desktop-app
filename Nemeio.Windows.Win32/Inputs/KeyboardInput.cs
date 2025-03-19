using System;
using System.Runtime.InteropServices;

namespace Nemeio.Windows.Win32.Inputs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInput
    {
        public UInt16 KeyCode;
        public UInt16 Scan;
        public UInt32 Flags;
        public UInt32 Time;
        public IntPtr ExtraInfo;
    }
}
