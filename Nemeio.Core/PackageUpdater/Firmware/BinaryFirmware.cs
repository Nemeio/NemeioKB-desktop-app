using System;

namespace Nemeio.Core.PackageUpdater.Firmware
{
    public sealed class BinaryFirmware : IFirmware
    {
        private byte[] _content;

        public byte[] ToByteArray() => _content;

        public BinaryFirmware(byte[] content)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}
