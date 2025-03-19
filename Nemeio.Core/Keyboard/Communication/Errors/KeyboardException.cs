using System;
using Nemeio.Core.Keyboard.Communication.Errors;
using Nemeio.Core.Keyboard.Communication.Exceptions;

namespace Nemeio.Core.Keyboard.Errors
{
    [Serializable]
    public class KeyboardException : KeyboardCommunicationException
    {
        public KeyboardErrorCode ErrorCode { get; private set; }

        public KeyboardException(KeyboardErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public KeyboardException(KeyboardErrorCode errorCode, string message)
            : base($"{message} with error code <{errorCode}>")
        {
            ErrorCode = errorCode;
        }
    }
}
