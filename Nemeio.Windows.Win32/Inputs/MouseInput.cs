using System;

namespace Nemeio.Windows.Win32.Inputs
{
    public struct MouseInput
    {
        public Int32 X { get; }
        public Int32 Y { get; }
        public UInt32 MouseData { get; }
        public UInt32 Flags { get; }
        public UInt32 Time { get; }
        public IntPtr ExtraInfo { get; }
    }
}