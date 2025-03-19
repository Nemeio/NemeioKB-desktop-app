using System;

namespace Nemeio.Core.Tools.Retry
{
    public sealed class SyncRetryAction : RetryAction
    {
        private Action _action;

        public SyncRetryAction(string name, uint retryCount, Action action) 
            : base(name, retryCount)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Execute() => _action();
    }
}
