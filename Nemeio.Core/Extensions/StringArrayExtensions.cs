namespace Nemeio.Core.Extensions
{
    public static class StringArrayExtensions
    {
        public static string ToReadeableString(this string[] self)
        {
            var result = "";

            if (self == null)
            {
                return result;
            }

            foreach (var str in self)
            {
                result += str + " - ";
            }
            return result;
        }
    }
}
