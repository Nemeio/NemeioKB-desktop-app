using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppKit;
using CoreFoundation;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Platform.Mac.Layouts.Systems
{
    public class MacSystemLayoutLoaderAdapter : ISystemLayoutLoaderAdapter
    {
        public IEnumerable<OsLayoutId> Load()
        {
            IEnumerable<OsLayoutId> result = new List<OsLayoutId>();

            // Check when the current thread is on background against main
            if (Thread.CurrentThread.IsBackground == true)
            {
                DispatchQueue.MainQueue.DispatchSync(() =>
                {
                    result = GetSystemKeyboardLayoutsOnMainThread();
                });
            }
            else
            {
                result = GetSystemKeyboardLayoutsOnMainThread();
            }

            return result;
        }

        private IEnumerable<OsLayoutId> GetSystemKeyboardLayoutsOnMainThread()
        {
            var inputContext = new NSTextInputContext();

            return inputContext.KeyboardInputSources.Select(s => BuildMacOsLayoutIdOnMainThread(s)).ToList();
        }

        private MacOsLayoutId BuildMacOsLayoutIdOnMainThread(string source)
        {
            var name = NSTextInputContext.LocalizedNameForInputSource(source);

            return new MacOsLayoutId(source, name);
        }
    }
}
