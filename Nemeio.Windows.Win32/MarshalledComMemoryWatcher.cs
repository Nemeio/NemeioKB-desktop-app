using System;
using System.Runtime.InteropServices;

namespace Nemeio.Windows.Win32
{
    public class MarshalledComMemoryWatcher : IDisposable
    {
        public MarshalledComMemoryWatcher(int size)
        {
            MemoryBlock = Marshal.AllocCoTaskMem(size);
        }

        public IntPtr MemoryBlock { get; private set; }

        public void Dispose()
        {
            if (MemoryBlock != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(MemoryBlock);
                MemoryBlock = IntPtr.Zero;
            }
        }
    }
}
