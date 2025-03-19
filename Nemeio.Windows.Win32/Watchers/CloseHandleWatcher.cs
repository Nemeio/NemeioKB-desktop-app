using System;
using System.Runtime.InteropServices;

namespace Nemeio.Windows.Win32
{

    public class CloseHandleWatcher : IDisposable
    {
        #region API32

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        #endregion

        IntPtr _handle;

        public CloseHandleWatcher(IntPtr handle)
        {
            _handle = handle;
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                CloseHandle(_handle);
            }
        }
    }
}
