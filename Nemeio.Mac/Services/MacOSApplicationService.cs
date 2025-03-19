using System;
using AppKit;
using CoreFoundation;
using Nemeio.Core.Services;
using Nemeio.Core.Tools.Dispatcher;

namespace Nemeio.Mac.Services
{
    public class MacOSApplicationService : IApplicationService
    {
        public IDispatcher GetMainThreadDispatcher()
        {
            throw new NotImplementedException();
        }

        public void StopApplication()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                NSApplication.SharedApplication.Terminate(null);
            });
        }
    }
}
