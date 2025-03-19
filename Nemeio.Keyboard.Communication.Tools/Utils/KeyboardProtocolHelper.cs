using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;
using Nemeio.Keyboard.Communication.Tools.Frames;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nemeio.Keyboard.Communication.Tools.Utils
{
    public static class KeyboardProtocolHelpers
    {
        public const int MASK_FO = 0xF0;
        public const int SHIFT_4 = 4;
        public static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public static byte[] GetBytes(this LayoutInfoDto layoutInfoDto)
        {
            var str = JsonConvert.SerializeObject(layoutInfoDto, jsonSerializerSettings);
            return GetBytes(str);
        }

        public static byte[] GetBytes(string input) => System.Text.Encoding.UTF8.GetBytes(input);

        public static byte[] GetBytes(short input) => ToBigEndian(BitConverter.GetBytes(input), 2);

        public static byte[] GetBytes(ushort input) => ToBigEndian(BitConverter.GetBytes(input), 2);

        public static byte[] GetBytes(uint input) => ToBigEndian(BitConverter.GetBytes(input), 4);

        public static byte[] GetBytes(int input) => ToBigEndian(BitConverter.GetBytes(input), 4);

        public static int ToInt32(byte[] input) => BitConverter.ToInt32(ToBigEndian(input, 4), 0);

        public static uint ToUInt32(byte[] input) => BitConverter.ToUInt32(ToBigEndian(input, 4), 0);

        public static uint ToUInt32LE(byte[] input) => BitConverter.ToUInt32(ToLittleEndian(input, 4), 0);

        public static ushort ToUInt16(byte[] input) => BitConverter.ToUInt16(ToBigEndian(input, 2), 0);

        public static byte[] ToBigEndian(byte[] input, int size)
        {
            if (!BitConverter.IsLittleEndian) return input;

            var copy = input.Copy(0, size);
            Array.Reverse(copy);
            return copy;
        }

        public static byte[] ToLittleEndian(byte[] input, int size)
        {
            if (BitConverter.IsLittleEndian) return input;

            var copy = input.Copy(0, size);
            Array.Reverse(copy);
            return copy;
        }

        public static byte[] Copy(this byte[] source) => Copy(source, 0, source.Length);

        public static byte[] Copy(this byte[] source, int offset, int size)
        {
            var copy = new byte[size];
            Array.Copy(source, offset, copy, 0, size);
            return copy;
        }

        public static NemeioKeyboardPacket GetNemeioPacket(byte[] packetBytes)
        {
            using (var mPacket = new MemoryStream(packetBytes))
            using (var bPacket = new BinaryReader(mPacket))
            {
                var header = bPacket.ReadUInt16();
                var command = bPacket.ReadSByte();
                var lengthBytes = bPacket.ReadBytes(2);
                if (BitConverter.IsLittleEndian) { Array.Reverse(lengthBytes); }
                var length = BitConverter.ToInt16(lengthBytes, 0);

                var packet = new NemeioKeyboardPacket
                {
                    Header = header,
                    Command = command,
                    Length = length
                };

                var payload = bPacket.ReadBytes(packet.Length);
                packet.Keystrokes = GetNemeioKeystrokes(payload);

                return packet;
            }
        }

        public static NemeioIndexKeystroke[] GetNemeioKeystrokes(byte[] packetBytes)
        {
            using (var mPayload = new MemoryStream(packetBytes))
            using (var bPayload = new BinaryReader(mPayload))
            {
                sbyte count = bPayload.ReadSByte();
                if (count <= 0) { return new NemeioIndexKeystroke[0]; }

                var keystrokes = new List<int>();
                int pDescriptor = 0;
                do
                {
                    keystrokes.Add(bPayload.GetInt());
                    pDescriptor++;
                } while (pDescriptor < count);

                return keystrokes.Select(k => new NemeioIndexKeystroke { Index = k }).ToArray();
            }
        }

        private static uint[] ParseKeyboardRegistries(BinaryReader reader)
        {
            uint[] registries = new uint[KeyboardFailure.NumberOfRegistries];
            for (int i = 0; i < KeyboardFailure.NumberOfRegistries; i++)
            {
                registries[i] = reader.ReadUInt32();
            }
            return registries;
        }

        private static KeyboardFailure ParseAssertFailEvent(BinaryReader reader)
        {
            return new KeyboardFailure(KeyboardEventId.AssertFailEvent, ParseKeyboardRegistries(reader),
                reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
        }

        private static KeyboardFailure ParseFaultExceptionEvent(BinaryReader reader)
        {
            return new KeyboardFailure(KeyboardEventId.FaultExceptionEvent, ParseKeyboardRegistries(reader),
                reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32(),
                (KeyboardExceptionEventTypeId)reader.ReadUInt32());
        }
        private static KeyboardFailure ParseTestEvent(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            var test = System.Text.Encoding.Default.GetString(reader.ReadBytes(length));
            var rows = test.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return new KeyboardFailure(KeyboardEventId.TestEvent, rows);
        }

        private static KeyboardFailure ParseKeyboardLog(BinaryReader reader)
        {
            var id = (KeyboardEventId)reader.ReadByte();
            switch (id)
            {
                case KeyboardEventId.AssertFailEvent:
                    return ParseAssertFailEvent(reader);
                case KeyboardEventId.FaultExceptionEvent:
                    return ParseFaultExceptionEvent(reader);
                case KeyboardEventId.TestEvent:
                    return ParseTestEvent(reader);
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Method to parse into a KeyboardFailure, the internally stored SysFailLog data as received by
        /// SerialComm::ReceiveDataDispatcher. This is provided as an array of bytes according LittleEndian
        /// convention (intrenal keyboard - firmware - convention for file storage)
        /// </summary>
        /// <returns>Parsed instances of KeyboardFailure (More than one occurence possible)</returns>
        /// <exception cref="ArgumentOutOfRangeException">When provided data to be parsed does not correspond to expected data size</exception>
        static public IList<KeyboardFailure> ParseSysFailLogDataFromLittleEndianStream(byte[] dataToParse)
        {
            var result = new List<KeyboardFailure>();

            using (var mData = new MemoryStream(dataToParse))
            using (var bData = new BinaryReader(mData))
            {
                while (bData.BaseStream.Position != bData.BaseStream.Length)
                {
                    result.Add(ParseKeyboardLog(bData));
                }
            }
            return result;
        }

        // Why not use the Framework's GetInt32 ?
        private static int GetInt(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian) { Array.Reverse(bytes); }
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
