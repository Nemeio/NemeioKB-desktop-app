using System;
using System.Threading.Tasks;

namespace Nemeio.Core.Tools.Retry
{
    public sealed class AsyncRetryAction : RetryAction
    {
        private Func<Task> _asyncTask;

        public AsyncRetryAction(string name, uint retryCount, Func<Task> asyncTask) 
            : base(name, retryCount)
        {
            _asyncTask = asyncTask ?? throw new ArgumentNullException(nameof(asyncTask));
        }

        public Task ExecuteAsync() => _asyncTask();
    }
}
