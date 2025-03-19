using System;

namespace Nemeio.Core.Tools.Retry
{
    [Serializable]
    public sealed class RetryFailedException : Exception
    {
        public RetryFailedException(Exception innerException) 
            : base("Maximum retry count reached", innerException) { }
    }
}
