using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Layouts.Active.Exceptions
{
    public sealed class ActiveLayoutSynchronizationFailed : Exception
    {
        public ActiveLayoutSynchronizationFailed() { }

        public ActiveLayoutSynchronizationFailed(string message) 
            : base(message) { }

        public ActiveLayoutSynchronizationFailed(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public ActiveLayoutSynchronizationFailed(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
