using System.IO;
using Nemeio.Core.Resources;

namespace Nemeio.Windows.Application.Resources
{
    public sealed class WinResourceLoader : IResourceLoader
    {
        public Stream GetResourceStream(string name) => LayoutGen.Resources.Resources.GetResourceStream(name);
    }
}
