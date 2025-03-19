using System;
using System.Runtime.InteropServices;

namespace Nemeio.Windows.Win32.Inputs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInput
    {
        public UInt32 Msg;

        public UInt16 ParamL;

        public UInt16 ParamH;
    }
}
