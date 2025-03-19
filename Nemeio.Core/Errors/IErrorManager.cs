using System;
using System.Collections.Generic;

namespace Nemeio.Core.Errors
{
    public interface IErrorManager
    {
        IList<ErrorTrace> ErrorStack { get; }

        string GetErrorDescription(ErrorCode errorCode);
        string GetFullErrorMessage(ErrorCode errorCode, Exception exception = null);
    }
}
