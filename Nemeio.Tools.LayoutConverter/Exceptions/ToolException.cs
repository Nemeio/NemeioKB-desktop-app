using System;
using Nemeio.Tools.LayoutConverter.Models;

namespace Nemeio.Tools.LayoutConverter.Exceptions
{
    internal class ToolException : Exception
    {
        public string AdditionalInformation { get; private set; }
        public ToolErrorCode ErrorCode { get; private set; }

        internal ToolException(ToolErrorCode errorCode, string additionalInformation = null)
        {
            ErrorCode = errorCode;
            AdditionalInformation = additionalInformation;
        }
    }
}
