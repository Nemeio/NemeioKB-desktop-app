using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Utils;
using Nemeio.Core;
using Nemeio.Core.Keyboard.Communication.Exceptions;

namespace Nemeio.Keyboard.Communication.Tools.Frames
{
    public class SerialFrame : IFrame
    {
        private const int StartSize = sizeof(byte);
        private const int CommandIdSize = sizeof(byte);
        private const int PayloadSize = sizeof(ushort);
        private const int CrcSize = sizeof(int);
        private const int EndSize = sizeof(byte);
        private const int MaxFrameSize = 1024;
        public const int MaxSendDataChunk = MaxFrameSize - StartSize - CommandIdSize - PayloadSize - CrcSize - EndSize - sizeof(byte) - sizeof(byte) - sizeof(int);

        private static readonly byte[] _startOfFrame = new byte[] { 0x01 };
        private static readonly byte[] _endOfFrame = new byte[] { 0x04 };

        public CommandId CommandId { get; }
        public byte[] Payload { get; }
        public uint Crc { get; }
        public byte[] Bytes { get; }

        public static List<IFrame> Parse(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var frames = new List<IFrame>();
            SerialFrame frame;
            while ((frame = Parse(stream)) != null)
            {
                frames.Add(frame);
            }
            return frames;
        }

        private static SerialFrame Parse(MemoryStream stream)
        {
            var header = ReadHeader(stream);
            if (header.Length == 0) return null;
            ValidateSof(header);

            var cmdId = ParseCommandId(header);
            var body = ReadBody(header, stream);
            var payload = body.Copy(0, body.Length - CrcSize - EndSize);
            var crc = ParseCrc(header[1], payload, body);
            ValidateEof(body);

            return new SerialFrame(cmdId, payload, crc);
        }

        private static byte[] ReadHeader(MemoryStream stream)
        {
            int count = 0;
            var header = new byte[StartSize + CommandIdSize + PayloadSize];
            count = stream.Read(header, 0, header.Length);
            return (count != 0) ? header : new byte[0];
        }

        private static void ValidateSof(byte[] header)
        {
            var sof = header.Copy(0, StartSize);
            if (!sof.SequenceEqual(_startOfFrame)) throw new InvalidFrameException();
        }

        private static CommandId ParseCommandId(byte[] header)
        {
            var input = header.Copy(StartSize, CommandIdSize);
            if (input.Length != CommandIdSize) throw new InvalidFrameException();
            var cmd = input[0] & ~0x80;
            if (!Enum.IsDefined(typeof(CommandId), (byte)cmd)) throw new InvalidFrameException();
            return (CommandId)cmd;
        }

        private static byte[] ReadBody(byte[] header, MemoryStream stream)
        {
            var payloadSizeRaw = header.Copy(StartSize + CommandIdSize, PayloadSize);
            var payloadSize = KeyboardProtocolHelpers.ToUInt16(payloadSizeRaw);
            var body = new byte[payloadSize + CrcSize + EndSize];
            // issue fix where serial frame was hiding innapropriate frame size, firing later on an InvalidFrameException
            if (body.Length > (stream.Length - stream.Position))
            {
                throw new ArgumentOutOfRangeException();
            }
            stream.Read(body, 0, body.Length);
            return body;
        }

        private static uint ParseCrc(byte cmdId, byte[] payload, byte[] body)
        {
            var existing = body.Copy(body.Length - CrcSize - EndSize, CrcSize);
            var computed = ComputeCrc(cmdId, payload);

            if (KeyboardProtocolHelpers.ToUInt32(existing) != computed) throw new InvalidFrameException();
            return computed;
        }

        private static uint ComputeCrc(byte commandId, byte[] body)
        {
            var bytes = new[] { commandId }
                .Append(KeyboardProtocolHelpers.GetBytes((short)body.Length))
                .Append(body);

            return Force.Crc32.Crc32Algorithm.Compute(bytes);
        }

        private static void ValidateEof(byte[] body)
        {
            var eof = body.Copy(body.Length - EndSize, EndSize);

            if (!eof.SequenceEqual(_endOfFrame))
            {
                throw new InvalidFrameException();
            }
        }

        public SerialFrame(CommandId commandId, byte[] payload)
            : this(commandId, payload, ComputeCrc((byte)commandId, payload)) { }

        private SerialFrame(CommandId commandId, byte[] payload, uint crc)
        {
            CommandId = commandId;
            Payload = payload;
            Crc = crc;
            Bytes = GetBytes();
        }

        private byte[] GetBytes() => _startOfFrame
            .Append(new byte[] { (byte)CommandId })
            .Append(KeyboardProtocolHelpers.GetBytes((short)Payload.Length))
            .Append(Payload)
            .Append(KeyboardProtocolHelpers.GetBytes(Crc))
            .Append(_endOfFrame);
    }
}
