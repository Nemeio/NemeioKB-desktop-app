using System.Collections.Generic;

namespace Nemeio.Core.FileSystem
{
    public sealed class Directory : FileSystemEntry, IDirectory
    {
        public Directory(string name, string path) 
            : base(name, path) { }

        public IEnumerable<IFileSystemEntry> GetEntries()
        {
            var content = new List<IFileSystemEntry>();

            foreach (var fileEntry in System.IO.Directory.EnumerateFiles(Path))
            {
                var file = new File(
                    System.IO.Path.GetFileName(fileEntry),
                    fileEntry
                );

                content.Add(file);
            }

            foreach (var directoryEntry in System.IO.Directory.EnumerateDirectories(Path))
            {
                var directory = new Directory(
                    System.IO.Path.GetFileName(directoryEntry),
                    directoryEntry
                );

                content.Add(directory);
            }

            return content;
        }
    }
}
