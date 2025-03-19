using System;

namespace Nemeio.Core.Applications.Manifest
{
    public class FirmwareInformation
    {
        public Version Version { get; private set; }
        public string FileName { get; private set; }

        public FirmwareInformation(Version version, string fileName)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            FileName = fileName;
        }
    }
}
