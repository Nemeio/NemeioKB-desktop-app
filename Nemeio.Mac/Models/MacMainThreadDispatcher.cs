using System;
using Nemeio.Core.Tools.Dispatcher;
using Nemeio.Platform.Mac.Utils;

namespace Nemeio.Mac.Models
{
    public class MacMainThreadDispatcher : IDispatcher
    {
        public void Invoke(Action action) => DispatchQueueUtils.DispatchAsyncOnMainQueueIfNeeded(action);
    }
}
