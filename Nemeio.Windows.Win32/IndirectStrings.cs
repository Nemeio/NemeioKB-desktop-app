using System.Runtime.InteropServices;
using System.Text;

namespace Nemeio.Windows.Win32
{
    public class IndirectStrings
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, string ppvReserved);

        private const int Success = 0;

        public static string GetIndirectString(string indirectString)
        {
            const int DefaultStringSize = 1024;

            if (string.IsNullOrEmpty(indirectString))
            {
                return null;
            }

            try
            {
                StringBuilder stringBuilder = new StringBuilder(DefaultStringSize);
                var resultCode = SHLoadIndirectString(indirectString, stringBuilder, DefaultStringSize, null);
                if (resultCode == Success)
                {
                    return stringBuilder.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
