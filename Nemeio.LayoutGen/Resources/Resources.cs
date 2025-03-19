using System.IO;
using System.Linq;
using System.Reflection;

namespace Nemeio.LayoutGen.Resources
{
    public class Resources
    {
        public static Stream GetResourceStream(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceStream($"Nemeio.LayoutGen.Resources.{name}");
        }

        public static bool ResourceExists(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceNames().Contains($"Nemeio.LayoutGen.Resources.{name}");
        }
    }
}
