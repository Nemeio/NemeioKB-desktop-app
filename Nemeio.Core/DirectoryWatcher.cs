using System;
using System.Collections.Generic;
using System.IO;

namespace Nemeio.Core
{
    public class DirectoryWatcher : IDisposable
    {
        private readonly char[] _separators = new char[] { Path.DirectorySeparatorChar,  Path.AltDirectorySeparatorChar };
        private readonly List<string> _builtPaths = new List<string>();
        public string Folder { get; private set; }

        /// <summary>
        /// Base constructor to possibly create a missing folder and then remove it
        /// after use.
        /// </summary>
        /// <param name="path">Path to be created and watched for deletion after use</param>
        /// <exception cref="IOException">Regular CreateDirectoryException</exception>
        /// <exception cref="UnauthorizedAccessException">Regular CreateDirectoryException</exception>
        /// <exception cref="ArgumentException">Regular CreateDirectoryException</exception>
        /// <exception cref="PathTooLongException">Regular CreateDirectoryException</exception>
        /// <exception cref="NotSupportedException">Regular CreateDirectoryException</exception>
        public DirectoryWatcher(string path)
        {
            Folder = null;
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (File.Exists(path))
            {
                // a watched folder cannot be an existing file
                throw new InvalidOperationException($"Cannot create a folder from existing file <{path}>");
            }

            BuildAndStoreIntermediateFolders(path);
        }

        public void Dispose()
        {
            if (_builtPaths.Count>0)
            {
                Directory.Delete(_builtPaths[0], true);
                _builtPaths.Clear();
            }
            Folder = null;
        }

        private void BuildAndStoreIntermediateFolders(string path)
        {
            string[] directories = path.Split(_separators);
            string currentPath = string.Empty;
            foreach(var directoryName in directories)
            {
                currentPath = Path.Combine(currentPath, directoryName);
                if (currentPath.EndsWith(":"))
                {
                    currentPath += Path.DirectorySeparatorChar;
                }
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                    _builtPaths.Add(currentPath);
                }
            }

            var directoryInfo = Directory.CreateDirectory(path);
            Folder = directoryInfo.FullName;
        }
    }
}
