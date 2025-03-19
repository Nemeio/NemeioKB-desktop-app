using System;
using System.IO;
using Nemeio.Core.Resources;

namespace Nemeio.Mac.Application.Resources
{
    public class MacResourceLoader : IResourceLoader
    {
        public Stream GetResourceStream(string name) => LayoutGen.Resources.Resources.GetResourceStream(name);
    }
}
