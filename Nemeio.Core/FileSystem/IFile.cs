using System.Threading.Tasks;

namespace Nemeio.Core.FileSystem
{
    public interface IFile : IFileSystemEntry
    {
        string Extension { get; }

        Task<string> ReadContentAsync();
        Task<byte[]> ReadByteArrayContentAsync();
    }
}
