using System;
using System.Runtime.Serialization;

namespace Nemeio.Core.Keyboard.DisplayCheckerBoard
{
    [Serializable]
    public class DisplayCheckerBoardFailedException : Exception
    {
        public DisplayCheckerBoardFailedException() { }

        public DisplayCheckerBoardFailedException(string message)
            : base(message) { }

        public DisplayCheckerBoardFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        protected DisplayCheckerBoardFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
