using System.Collections.Generic;

namespace Nemeio.Core.FileSystem
{
    public interface IDirectory : IFileSystemEntry
    {
        IEnumerable<IFileSystemEntry> GetEntries();
    }
}
