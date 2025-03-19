using System;
using System.IO;

namespace Nemeio.Core
{
    public class FileWatcher : IDisposable
    {
        private DirectoryWatcher _folderWatcher;
        public string FullPath { get; private set; }

        public FileWatcher(string path)
        {
            FullPath = null;
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (Directory.Exists(path))
            {
                // a watched file must have a name and cannot be ane xsiting folder
                throw new InvalidOperationException($"Cannot create a file with existing folder name <{path}>");
            }

            if (!File.Exists(path))
            {
                var folder = Path.GetDirectoryName(path);
                if (!Directory.Exists(folder))
                {
                    _folderWatcher = new DirectoryWatcher(folder);
                }

                var stream = File.Create(path);
                stream.Close();
            }
            FullPath = path;
        }

        public void Dispose()
        {
            if (!string.IsNullOrWhiteSpace(FullPath) &&
                File.Exists(FullPath))
            {
                File.Delete(FullPath);
                FullPath = null;
            }
            if (_folderWatcher != null)
            {
                _folderWatcher.Dispose();
                _folderWatcher = null;
            }
        }
    }

}
