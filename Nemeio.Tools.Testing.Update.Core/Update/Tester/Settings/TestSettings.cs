using System;

namespace Nemeio.Tools.Testing.Update.Core.Update.Tester.Settings
{
    public class TestSettings
    {
        public UpdateVersionRange VersionRange { get; private set; }
        public string EnvironmentName { get; private set; }
        public Uri ServerUri { get; private set; }
        public string OutputPath { get; private set; }

        public TestSettings(UpdateVersionRange versionRange, string environmentName, Uri serverUri, string outputPath)
        {
            VersionRange = versionRange ?? throw new ArgumentNullException(nameof(versionRange));

            if (string.IsNullOrWhiteSpace(environmentName))
            {
                throw new ArgumentNullException(nameof(environmentName));
            }

            EnvironmentName = environmentName;
            ServerUri = serverUri ?? throw new ArgumentNullException(nameof(serverUri));
            OutputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
        }
    }
}
