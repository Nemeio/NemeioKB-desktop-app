using Nemeio.Core.Device;
using Nemeio.Core.Keyboard.Communication.Commands;
using Nemeio.Core.Keyboard.Communication.Frame;
using Nemeio.Keyboard.Communication.Tools.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Nemeio.Keyboard.Communication.Protocol.v1.Commands
{
    public class SetProvisionningCommand : KeyboardCommand
    {
        private readonly string _serial;
        private readonly PhysicalAddress _mac;

        public SetProvisionningCommand(string serial, PhysicalAddress mac)
            : base(CommandId.SetProvisionning)
        {
            _serial = serial;
            _mac = mac;

            Timeout = new TimeSpan(0, 0, 5);
        }

        public override IList<IFrame> ToFrames()
        {
            var serialPayload = Encoding.ASCII.GetBytes(_serial);
            var macPayload = (_mac.GetAddressBytes());
            var payload = serialPayload.Concat(macPayload).ToArray();

            return new List<IFrame>() { new SerialFrame(CommandId.SetProvisionning, payload) };
        }
    }
}
