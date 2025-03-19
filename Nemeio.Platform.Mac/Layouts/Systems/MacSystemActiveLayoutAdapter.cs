using System;
using System.Threading;
using AppKit;
using CoreFoundation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems;

namespace Nemeio.Platform.Mac.Layouts.Systems
{
    public class MacSystemActiveLayoutAdapter : ISystemActiveLayoutAdapter
    {
        private readonly ILogger _logger;

        public MacSystemActiveLayoutAdapter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MacSystemActiveLayoutAdapter>();
        }

        public event EventHandler OnSystemActionLayoutChanged;

        public OsLayoutId GetCurrentSystemLayoutId()
        {
            MacOsLayoutId result = null;

            // Check whethercurrentthread is on background against main
            if (Thread.CurrentThread.IsBackground == true)
            {
                DispatchQueue.MainQueue.DispatchSync(() =>
                {
                    result = GetCurrentSystemRawLayoutIdOnMainThread();
                });
            }
            else
            {
                result = GetCurrentSystemRawLayoutIdOnMainThread();
            }


            return result;
        }

        public OsLayoutId GetDefaultSystemLayoutId()
        {
            // TODO JVL : retrieve default layout for mac.
            return GetCurrentSystemLayoutId();
        }

        private MacOsLayoutId GetCurrentSystemRawLayoutIdOnMainThread()
        {
            var inputContext = new NSTextInputContext();
            var source = inputContext.SelectedKeyboardInputSource;
            var name = NSTextInputContext.LocalizedNameForInputSource(source);

            return new MacOsLayoutId(source, name);
        }
    }
}
