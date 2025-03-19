using System;

namespace Nemeio.Keyboard.Communication.Linux.Watchers
{
    internal sealed class DeviceHolder
    {
        public string Identifier { get; private set; }
        public string Name { get; private set; }
        public Version Version { get; private set; }

        public DeviceHolder(Core.Keyboard.Keyboard keyboard, string name)
            : this(keyboard.Identifier, name, keyboard.ProtocolVersion) { }

        public DeviceHolder(string identifier, string name, Version version)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
    }
}
