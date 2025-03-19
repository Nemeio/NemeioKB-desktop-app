using System;

namespace Nemeio.LayoutGen.Exceptions
{
    public class FontNotFoundException : Exception
    {
        public FontNotFoundException()
        {

        }

        public FontNotFoundException(string message) : base(message)
        {

        }

        public FontNotFoundException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
