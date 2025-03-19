using System.IO;

namespace Nemeio.Core.PackageUpdater.Firmware
{
    public interface IFirmware
    {
        byte[] ToByteArray();
    }

    public interface IPackageFirmware : IFirmware
    {
        FirmwarePackageHeader GlobalHeader { get; set; }
        FirmwarePackageFirmwareHeader Stm32Header { get; set; }
        FirmwarePackageFirmwareHeader NrfHeader { get; set; }
        FirmwarePackageFirmwareHeader IteHeader { get; set; }
        byte[] Stm32Firmware { get; set; }
        byte[] NrfFirmware { get; set; }
        byte[] IteFirmware { get; set; }

        void Write(BinaryWriter writer);
    }

    public sealed class PackageFirmware : IPackageFirmware
    {
        public FirmwarePackageHeader GlobalHeader { get; set; } = new FirmwarePackageHeader();

        public FirmwarePackageFirmwareHeader Stm32Header { get; set; } = new FirmwarePackageFirmwareHeader();

        public FirmwarePackageFirmwareHeader NrfHeader { get; set; } = new FirmwarePackageFirmwareHeader();

        public FirmwarePackageFirmwareHeader IteHeader { get; set; } = new FirmwarePackageFirmwareHeader();

        public byte[] Stm32Firmware { get; set; }

        public byte[] NrfFirmware { get; set; }

        public byte[] IteFirmware { get; set; }

        public void Write(BinaryWriter writer)
        {
            GlobalHeader.Write(writer);
            Stm32Header.Write(writer);
            NrfHeader.Write(writer);
            IteHeader.Write(writer);
            writer.Write(Stm32Firmware);
            writer.Write(NrfFirmware);
            writer.Write(IteFirmware);
        }

        public void Read(BinaryReader reader)
        {
            GlobalHeader.Read(reader);
            Stm32Header.Read(reader);
            NrfHeader.Read(reader);
            IteHeader.Read(reader);
            Stm32Firmware = reader.ReadBytes(Stm32Header.Size);
            NrfFirmware = reader.ReadBytes(NrfHeader.Size);
            IteFirmware = reader.ReadBytes(IteHeader.Size);
        }

        public byte[] ToByteArray()
        {
            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                Write(binaryWriter);

                return memoryStream.ToArray();
            }
        }
    }

    public sealed class FirmwarePackageHeader
    {
        private const int SignatureSize = 32;

        public uint Tag { get; set; }

        public byte FormatVersion { get; set; }

        public int Size { get; set; }

        public byte[] Signature { get; set; } = new byte[SignatureSize];

        public void Write(BinaryWriter writer)
        {
            writer.Write(Tag);
            writer.Write(FormatVersion);
            writer.Write(Size);
            writer.Write(Signature);
        }

        public void Read(BinaryReader reader)
        {
            Tag = reader.ReadUInt32();
            FormatVersion = reader.ReadByte();
            Size = reader.ReadInt32();
            Signature = reader.ReadBytes(SignatureSize);
        }
    }

    public enum PackageCompressionType : byte
    {
        None = 0x00,
        GZip = 0x01,
        Lz4 = 0x02,
    }

    public sealed class FirmwarePackageFirmwareHeader
    {
        public PackageCompressionType CompressionType { get; set; }

        public byte MajorVersion { get; set; }

        public byte MinorVersion { get; set; }

        public byte RevisionVersion { get; set; }

        public int BuildVersion { get; set; }

        public int Offset { get; set; }

        public int Size { get; set; }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)CompressionType);
            writer.Write(MajorVersion);
            writer.Write(MinorVersion);
            writer.Write(RevisionVersion);
            writer.Write(BuildVersion);
            writer.Write(Offset);
            writer.Write(Size);
        }

        public void Read(BinaryReader reader)
        {
            CompressionType = (PackageCompressionType)reader.ReadByte();
            MajorVersion = reader.ReadByte();
            MinorVersion = reader.ReadByte();
            RevisionVersion = reader.ReadByte();
            BuildVersion = reader.ReadInt32();
            Offset = reader.ReadInt32();
            Size = reader.ReadInt32();
        }
    }
}
