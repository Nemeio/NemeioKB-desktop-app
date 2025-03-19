using System;

namespace Nemeio.Api.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToTimestamp(this DateTime dt)
        {
            try
            {
                var unixTime = (DateTimeOffset)dt;

                return unixTime.ToUnixTimeSeconds();
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }
    }
}
