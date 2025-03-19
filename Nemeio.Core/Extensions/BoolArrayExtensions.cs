namespace Nemeio.Core.Extensions
{
    public static class BoolArrayExtensions
    {
        #if DEBUG

        public static string ToReadeableString(this bool[] self)
        {
            var result = "";

            if (self == null)
            {
                return result;
            }

            foreach (var b in self)
            {
                result += b.ToString() + " - ";
            }
            return result;
        }

        #endif
    }
}
