using System.IO;
using System.Reflection;
using SkiaSharp;

namespace Nemeio.LayoutGen.Test
{
    public static class FileHelper
    {
        public static byte[] ByteArrayFromResources(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format("Nemeio.LayoutGen.Test.Resources.{0}", name);

            using (var stream = asm.GetManifestResourceStream(resource))
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }

        public static Stream StreamFromResources(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resource = string.Format("Nemeio.LayoutGen.Test.Resources.{0}", name);

            return asm.GetManifestResourceStream(resource);
        }

        public static SKBitmap ReadPNGFromResources(string name)
        {
            var byteArray = ByteArrayFromResources(name);
            return SKBitmap.Decode(byteArray);
        }
    }
}
