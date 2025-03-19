using System;

namespace Nemeio.Core.Errors
{
    public class ErrorDescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public ErrorDescriptionAttribute(string errorDescription)
        {
            Description = errorDescription;
        }
    }
}
