using System;

namespace Nemeio.Core.FileSystem
{
    public abstract class FileSystemEntry : IFileSystemEntry
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public FileSystemEntry(string name, string path)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }
}
