using System;
using Nemeio.Core.Keyboard.Communication;

namespace Nemeio.Core.Keyboard
{
    public class Keyboard
    {
        public CommunicationType Communication { get; private set; }
        public System.Version ProtocolVersion { get; private set; }
        public string Identifier { get; private set; }
        public IKeyboardIO IO { get; private set; }

        public Keyboard(string identifier, System.Version protocolVersion, CommunicationType communication, IKeyboardIO io)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            Identifier = identifier;
            ProtocolVersion = protocolVersion ?? throw new ArgumentNullException(nameof(protocolVersion));
            IO = io ?? throw new ArgumentNullException(nameof(io));
            Communication = communication;
        }
    }
}
