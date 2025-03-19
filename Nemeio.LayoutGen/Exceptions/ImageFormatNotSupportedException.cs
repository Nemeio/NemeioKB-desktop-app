using System;

namespace Nemeio.LayoutGen.Exceptions
{
    public class ImageFormatNotSupportedException : Exception
    {
        public ImageFormatNotSupportedException()
        {

        }
        
        public ImageFormatNotSupportedException(string message) : base(message)
        {

        }

        public ImageFormatNotSupportedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
