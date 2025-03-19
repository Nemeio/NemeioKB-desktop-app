using System.Threading;
using AppKit;
using CoreFoundation;
using Nemeio.Core.Layouts.Name;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Platform.Mac.Layouts
{
    public class MacLayoutNativeNameAdapter : ILayoutNativeNameAdapter
    {
        public string RetrieveNativeName(OsLayoutId osLayoutId)
        {
            var layoutName = string.Empty;
            if (Thread.CurrentThread.IsBackground == true)
            {
                DispatchQueue.MainQueue.DispatchSync(() =>
                {
                    layoutName = NSTextInputContext.LocalizedNameForInputSource(osLayoutId);
                });
            }
            else
            {
                layoutName = NSTextInputContext.LocalizedNameForInputSource(osLayoutId);
            }

            return layoutName;
        }
    }
}
