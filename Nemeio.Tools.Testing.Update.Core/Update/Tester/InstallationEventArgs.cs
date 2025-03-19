using System;

namespace Nemeio.Tools.Testing.Update.Core.Update.Tester
{
    public class InstallationEventArgs
    {
        public Version Version { get; private set; }

        public InstallationEventArgs(Version version)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
    }
}
