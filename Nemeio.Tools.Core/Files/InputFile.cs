using System;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;

namespace Nemeio.Tools.Core.Files
{
    public abstract class InputFile
    {
        protected readonly IFileSystem _fileSystem;

        public string FilePath { get; private set; }

        public InputFile(IFileSystem fileSystem, string filePath)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            _fileSystem = fileSystem;

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!fileSystem.FileExists(filePath))
            {
                throw new InvalidOperationException($"File at path <{filePath}> doesn't exists");
            }

            FilePath = filePath;
        }

        public async Task<string> ReadContentAsync()
        {
            var content = await _fileSystem.ReadTextAsync(FilePath);

            return content;
        }

        public async Task<byte[]> ReadBinaryContentAsync()
        {
            var content = await _fileSystem.ReadByteArrayAsync(FilePath);

            return content;
        }
    }
}
