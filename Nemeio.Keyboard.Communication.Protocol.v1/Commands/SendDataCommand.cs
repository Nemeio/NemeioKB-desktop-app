using System.Collections.Generic;
using System.Linq;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Keyboard.Communication.Tools.Frames;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public abstract class SendDataCommand : KeyboardCommand
    {
        protected enum FrameDataType : byte
        {
            Firmware = 1,
            DefaultConfiguration = 2,
            Configuration = 3,
            DefaultWallpaper = 4,
            Wallpaper = 5,
            FirmwarePackage = 8,
        }

        protected enum SendCommandType : byte
        {
            Start = 0,
            Send = 1,
            Stop = 2,
        }

        public SendDataCommand()
            : base(CommandId.SendData) { }

        protected ICollection<SerialFrame> CreateSendDataPayloads(FrameDataType frameDataType, byte[] data)
            => new List<byte[]>()
                .AddChainable(GetStart(frameDataType, data.Length))
                .AddChainable(GetPulp(frameDataType, data))
                .AddChainable(GetEnd(frameDataType))
                .Select(p => new SerialFrame(CommandId, p)).ToArray();

        protected byte[] GetStart(FrameDataType frameDataType, int dataSize)
            => new[]
                { (byte)frameDataType, (byte)SendCommandType.Start }
                .Append(KeyboardProtocolHelpers.GetBytes(dataSize));

        protected IEnumerable<byte[]> GetPulp(FrameDataType frameDataType, byte[] data)
        {
            var payloads = Partition(data);
            var send = new[] { (byte)frameDataType, (byte)SendCommandType.Send };
            return payloads.Select(p => send.Append(p));
        }

        private byte[] GetEnd(FrameDataType frameDataType)
            => new[] { (byte)frameDataType, (byte)SendCommandType.Stop };

        private IEnumerable<byte[]> Partition(byte[] data)
        {
            var payloads = new List<byte[]>();
            var max = SerialFrame.MaxSendDataChunk;
            var nb = data.Length / max;

            for (int i = 0; i < nb; i++)
            {
                payloads.Add(GetPayload(data, i * max, max));
            }

            var rem = data.Length % max;
            if (rem != 0)
            {
                payloads.Add(GetPayload(data, nb * max, rem));
            }

            return payloads;
        }

        private byte[] GetPayload(byte[] data, int offset, int chunkSize)
        {
            var chunk = data.Copy(offset, chunkSize);
            var payload = KeyboardProtocolHelpers.GetBytes(offset).Append(chunk);
            return payload;
        }
    }
}
