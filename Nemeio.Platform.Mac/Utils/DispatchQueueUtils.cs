using System;
using CoreFoundation;

namespace Nemeio.Platform.Mac.Utils
{
    public static class DispatchQueueUtils
    {
        public static void DispatchSyncOnMainQueueIfNeeded(Action action)
        {
            if (DispatchQueue.CurrentQueue == DispatchQueue.MainQueue)
            {
                action();
            }
            else
            {
                DispatchQueue.MainQueue.DispatchSync(() =>
                {
                    action();
                });
            }
        }

        public static void DispatchAsyncOnMainQueueIfNeeded(Action action)
        {
            if (DispatchQueue.CurrentQueue == DispatchQueue.MainQueue)
            {
                action();
            }
            else
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    action();
                });
            }
        }
    }
}
