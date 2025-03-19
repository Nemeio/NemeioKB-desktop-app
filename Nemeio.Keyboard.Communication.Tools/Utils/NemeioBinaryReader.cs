using System;
using System.Collections.Generic;
using Nemeio.Core;

namespace Nemeio.Keyboard.Communication.Tools.Utils
{
    public enum SignificantByte
    {
        MSB,
        LSB
    }

    public class NemeioBinaryReader
    {
        private byte[] _data;

        public NemeioBinaryReader(byte[] data) => _data = data;

        public uint ReadUInt32(SignificantByte significant = SignificantByte.MSB) => BitConverter.ToUInt32(
            GetSignificantForSize(sizeof(uint), significant),
            0
        );

        public ushort ReadUInt16(SignificantByte significant = SignificantByte.MSB) => BitConverter.ToUInt16(
            GetSignificantForSize(sizeof(ushort), significant),
            0
        );

        public bool ReadBoolean(SignificantByte significant = SignificantByte.MSB) => BitConverter.ToBoolean(
            GetSignificantForSize(sizeof(bool), significant),
            0
        );

        public byte ReadByte(SignificantByte significant = SignificantByte.MSB) => GetSignificantForSize(sizeof(byte), significant)[0];

        public IList<byte> ReadByteList(int length, SignificantByte significant = SignificantByte.MSB)
        {
            var byteList = new List<byte>();

            for (var i = 0; i < length; i++)
            {
                var value = ReadByte(significant);

                byteList.Add(value);
            }

            return byteList;
        }

        private byte[] GetSignificantForSize(Int32 size, SignificantByte significant = SignificantByte.MSB)
        {
            var data = GetSignificantArray(
                _data.SubArray(0, size),
                significant
            );

            UpdateLocalData(size);

            return data;
        }

        private void UpdateLocalData(Int32 size) => _data = _data.SubArray(size, _data.Length - size);

        private byte[] GetSignificantArray(byte[] data, SignificantByte significant)
        {
            switch (significant)
            {
                case SignificantByte.MSB:
                    return KeyboardProtocolHelpers.ToBigEndian(data, data.Length);
                case SignificantByte.LSB:
                    return KeyboardProtocolHelpers.ToLittleEndian(data, data.Length);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
