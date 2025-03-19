using System.IO;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Core.Images.Jpeg
{
    public sealed class JpegImageHeader : IBinaryConvertible
    {
        public PackageCompressionType CompressionType { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public byte MajorVersion { get; set; } = 0;
        public byte MinorVersion { get; set; } = 0;
        public byte RevisionVersion { get; set; } = 0;
        public int BuildVersion { get; set; } = 0;

        public JpegImageHeader(PackageCompressionType compressionType, int offset, int size)
        {
            CompressionType = compressionType;
            Offset = offset;
            Size = size;
        }

        public static int HeaderSize => sizeof(byte) + sizeof(int) + sizeof(int) + sizeof(byte) + sizeof(byte) + sizeof(byte) + sizeof(int);

        public void Convert(BinaryWriter writer)
        {
            writer.Write((byte)CompressionType);
            writer.Write(MajorVersion);
            writer.Write(MinorVersion);
            writer.Write(RevisionVersion);
            writer.Write(BuildVersion);
            writer.Write(Offset);
            writer.Write(Size);
        }

        public int ComputeSize() => HeaderSize;
    }
}
