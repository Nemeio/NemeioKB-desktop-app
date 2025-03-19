using System;
using System.Collections.Generic;
using Nemeio.Core.Extensions;

namespace Nemeio.Core.Errors
{
    public class ErrorManager : IErrorManager
    {
        private const string DefaultErrorDescription = "Unknown error";

        public IList<ErrorTrace> ErrorStack { get; } = new List<ErrorTrace>();

        public string GetErrorDescription(ErrorCode errorCode)
        {
            var errorDescriptionAttribute = errorCode.GetAttribute<ErrorDescriptionAttribute>();
            if (errorDescriptionAttribute == null)
            {
                return DefaultErrorDescription;
            }

            return errorDescriptionAttribute.Description;
        }

        public string GetFullErrorMessage(ErrorCode errorCode, Exception exception = null)
        {
            var hexadecimalErrorCode = ((int)errorCode).ToString("X02");
            var errorCodeDescription    = GetErrorDescription(errorCode);

            var resultMessage = $"[{hexadecimalErrorCode}] {errorCodeDescription}";

            if (exception != null)
            {
                if (!string.IsNullOrEmpty(exception.Message))
                {
                    resultMessage += $"\n{exception.Message}";
                }

                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    resultMessage += $"\n{exception.StackTrace}";
                }
            }
            
            return resultMessage;
        }
    }
}
