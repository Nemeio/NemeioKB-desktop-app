using System;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Core.Images.Jpeg
{
    public sealed class JpegImageData
    {
        public byte[] Content { get; private set; }
        public PackageCompressionType CompressionType { get; private set; }

        public JpegImageData(byte[] content, PackageCompressionType compressionType)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            CompressionType = compressionType;
        }

        public static async Task<JpegImageData> FromFileAsync(IFileSystem fileSystem, string path, PackageCompressionType compressionType)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var content = await fileSystem.ReadByteArrayAsync(path);
            var imageFile = new JpegImageData(content, compressionType);

            return imageFile;
        }
    }
}
