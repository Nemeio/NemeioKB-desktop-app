using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.Builds
{
    [Serializable]
    public sealed class InitializationFailedException : Exception
    {
        public string Identifier { get; private set; }

        public InitializationFailedException() { }

        public InitializationFailedException(string identifier)
            : base($"Init failed with keyboard <{identifier}>") => Identifier = identifier;

        public InitializationFailedException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }

        public InitializationFailedException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
