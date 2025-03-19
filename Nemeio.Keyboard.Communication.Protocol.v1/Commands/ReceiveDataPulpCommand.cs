using System;
using System.Collections.Generic;
using Nemeio.Core;
using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Core.Keyboard.ReceiveData;
using Nemeio.Keyboard.Communication.Tools.Frames;
using Nemeio.Keyboard.Communication.Tools.Utils;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class ReceiveDataPulpCommand : KeyboardCommand
    {
        private IReceiveData _type;
        private uint _sizeToReceive;
        private uint _offset;

        public ReceiveDataPulpCommand(IReceiveData type, uint sizeToReceive, uint offset)
            : base(CommandId.ReceiveData)
        {
            _type = type;
            _sizeToReceive = sizeToReceive;
            _offset = offset;
        }

        public override IList<IFrame> ToFrames()
        {
            var dataChunkSize = Math.Min(_sizeToReceive - _offset, SerialFrame.MaxSendDataChunk);

            var payload = new[] { _type.Value, (byte)ReceiveDataCommandType.Receive }
                .Append(KeyboardProtocolHelpers.GetBytes(_offset))
                .Append(KeyboardProtocolHelpers.GetBytes(dataChunkSize));

            return new List<IFrame>() { new SerialFrame(CommandId, payload) };
        }
    }
}
