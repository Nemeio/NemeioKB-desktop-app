using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Nemeio.Core.PackageUpdater.Firmware;

namespace Nemeio.Core.Images.Jpeg
{
    public sealed class JpegImage : IBinaryConvertible
    {
        public JpegImageHeader Header { get; private set; }
        public byte[] Data { get; private set; }

        public JpegImage(JpegImageHeader header, byte[] data)
        {
            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            PadIfNeeded(ref data, ref header);

            Header = header;
            Data = CompressIfNeeded(data);
        }

        public int ComputeSize() => Data.Length;

        public void Convert(BinaryWriter writer)
        {
            writer.Write(Data);
        }

        private bool PadIfNeeded(ref byte[] data, ref JpegImageHeader header)
        {
            //  Check if length is odd
            if (data.Length % 2 == 1)
            {
                var tmpData = data.ToList();
                tmpData.Add(0);

                data = tmpData.ToArray();

                header.Size += 1;

                return true;
            }

            return false;
        }

        private byte[] CompressIfNeeded(byte[] data)
        {
            switch (Header.CompressionType)
            {
                case PackageCompressionType.None:
                    return data;

                case PackageCompressionType.GZip:
                    using (var compressStream = new MemoryStream(data))
                    using (var compressor = new GZipStream(compressStream, CompressionLevel.Optimal))
                    {
                        return compressStream.ToArray();
                    }

                default:
                    throw new InvalidOperationException($"Compression type {Header.CompressionType} not supported.");
            }
        }
    }
}
