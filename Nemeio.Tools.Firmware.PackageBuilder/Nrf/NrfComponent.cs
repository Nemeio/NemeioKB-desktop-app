using System;

namespace Nemeio.Tools.Firmware.PackageBuilder
{
    public class NrfComponent
    {
        public string BinFile { get; private set; }

        public string DatFile { get; private set; }

        public NrfComponent(string binFile, string datFile)
        {
            BinFile = binFile ?? throw new ArgumentNullException(nameof(binFile));
            DatFile = datFile ?? throw new ArgumentNullException(nameof(datFile));
        }
    }
}
