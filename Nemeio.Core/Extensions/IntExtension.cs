using System;

namespace Nemeio.Core.Extensions
{
    public static class IntExtension
    {
        public static byte ToByte(this int value)
        {
            return Convert.ToByte(value);
        }
    }
}
