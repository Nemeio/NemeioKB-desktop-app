using System.IO;

namespace Nemeio.Core.Images.Jpeg
{
    public sealed class JpegImagePackageHeader : IBinaryConvertible
    {
        public uint Tag { get; set; }
        public byte FormatVersion { get; set; }
        public int Size { get; set; }
        public byte[] Signature { get; set; } = new byte[32];
        public byte FirmwareCount { get; set; }

        public int ComputeSize() => sizeof(uint) + sizeof(byte) + sizeof(int) + Signature.Length + sizeof(byte);

        public void Convert(BinaryWriter writer)
        {
            writer.Write(Tag);
            writer.Write(FormatVersion);
            writer.Write(Size);
            writer.Write(Signature);
            writer.Write(FirmwareCount);
        }
    }
}
