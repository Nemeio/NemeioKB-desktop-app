using System.IO;

namespace Nemeio.Core.Resources
{
    public interface IResourceLoader
    {
        Stream GetResourceStream(string name);
    }
}
