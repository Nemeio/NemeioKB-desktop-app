using System;

namespace Nemeio.Tools.Firmware.PackageBuilder
{
    public class NrfManifest
    {
        public NrfComponent Application { get; private set; }

        public NrfComponent SoftDevice { get; private set; }

        public NrfManifest(NrfComponent application, NrfComponent softDevice)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
            SoftDevice = softDevice;
        }
    }
}
