using System;

namespace Nemeio.Core.Tools.Retry
{
    public abstract class RetryAction
    {
        public uint RetryCount { get; private set; }
        public string Name { get; private set; }

        public RetryAction(string name, uint retryCount)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RetryCount = retryCount;
        }
    }
}
