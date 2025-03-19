using System;

namespace Nemeio.Keyboard.Communication.Linux.CommandLine
{
    internal sealed class TtyDevice
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public TtyDevice(string name, string path)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }
}
