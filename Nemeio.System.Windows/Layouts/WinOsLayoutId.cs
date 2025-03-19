using System;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Platform.Windows.Layouts
{
    public class WinOsLayoutId : OsLayoutId
    {
        public IntPtr Handle { get; private set; }

        public WinOsLayoutId(string name, IntPtr handle)
            : base(handle.ToString(), name) 
        {
            Handle = handle;
        }

        public static implicit operator IntPtr(WinOsLayoutId value) => value.Handle;
    }
}
