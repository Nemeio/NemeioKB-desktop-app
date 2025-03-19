using System;
namespace Nemeio.Keyboard.Communication.Mac.Watchers
{
    public sealed class IoUsbDevice
    {
        public string Identifier { get; private set; }

        public int VendorId { get; private set; }

        public int ProductId { get; private set; }

        public Version Version { get; private set; }

        public IoUsbDevice(string identifier, int vendorId, int productId)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            VendorId = vendorId;
            ProductId = productId;
        }

        public IoUsbDevice(string identifier, int vendorId, int productId, Version version)
            : this(identifier, vendorId, productId)
        {
            Version = version;
        }
    }
}
