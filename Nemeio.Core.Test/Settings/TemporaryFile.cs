using System;
using System.IO;

namespace Nemeio.Core.Test.Settings
{
    internal sealed class TemporaryFile : IDisposable
    {
        private readonly string _filePath;

        public TemporaryFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            _filePath = filePath;
            
        }

        public void WriteAndClose(string content)
        {
            using (var stream = new StreamWriter(_filePath))
            {
                stream.WriteLine(content);
            }
        }

        public void Dispose()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}
