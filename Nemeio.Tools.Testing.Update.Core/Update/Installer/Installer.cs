using System;

namespace Nemeio.Tools.Testing.Update.Core.Update.Installer
{
    public class Installer
    {
        public string Url { get; private set; }
        public Version Version { get; private set; }

        public Installer(string url, Version version)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            Url = url;
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
    }
}
