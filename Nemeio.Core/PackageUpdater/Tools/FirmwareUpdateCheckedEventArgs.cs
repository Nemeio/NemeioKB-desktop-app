using System;

namespace Nemeio.Core.PackageUpdater.Tools
{
    public sealed class FirmwareUpdateCheckedEventArgs
    {
        public bool Found { get; private set; }
        public FirmwareUpdatableNemeioProxy Proxy { get; private set; }
        public FirmwareUpdateCheckedEventArgs(bool found, FirmwareUpdatableNemeioProxy proxy)
        {
            Found = found;
            Proxy = proxy;
        }
    }
}
