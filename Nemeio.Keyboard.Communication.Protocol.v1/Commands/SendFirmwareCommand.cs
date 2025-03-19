using System;
using System.Collections.Generic;
using Nemeio.Core.Keyboard.Communication.Frame;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class SendFirmwareCommand : SendDataCommand
    {
        private readonly byte[] _firmware;

        public SendFirmwareCommand(byte[] firmware)
        {
            _firmware = firmware ?? throw new ArgumentNullException(nameof(firmware));

            Timeout = new TimeSpan(0, 3, 0);
        }

        public override IList<IFrame> ToFrames()
        {
            var frames = CreateSendDataPayloads(FrameDataType.FirmwarePackage, _firmware);

            return new List<IFrame>(frames);
        }
    }
}
