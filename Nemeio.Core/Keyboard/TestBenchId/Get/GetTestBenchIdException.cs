using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.TestBenchId.Set
{
    [Serializable]
    public class GetTestBenchIdException : Exception
    {
        public GetTestBenchIdException() { }

        public GetTestBenchIdException(string message)
            : base(message) { }

        public GetTestBenchIdException(string message, Exception innerException)
            : base(message, innerException) { }

        protected GetTestBenchIdException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
