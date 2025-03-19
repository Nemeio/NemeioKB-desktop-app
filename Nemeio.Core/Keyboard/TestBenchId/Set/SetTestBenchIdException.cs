using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.TestBenchId.Set
{
    [Serializable]
    public class SetTestBenchIdException : Exception
    {
        public SetTestBenchIdException() { }

        public SetTestBenchIdException(string message)
            : base(message) { }

        public SetTestBenchIdException(string message, Exception innerException)
            : base(message, innerException) { }

        protected SetTestBenchIdException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
