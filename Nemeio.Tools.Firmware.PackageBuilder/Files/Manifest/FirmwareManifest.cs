﻿namespace Nemeio.Tools.Firmware.PackageBuilder.Files.Manifest
{
    public sealed class FirmwareManifest
    {
        public FirmwareInformation Cpu { get; set; }
        public FirmwareInformation BluetoothLE { get; set; }
        public FirmwareInformation Ite { get; set; }
    }
}
