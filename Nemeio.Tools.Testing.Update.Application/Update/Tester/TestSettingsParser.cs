using Nemeio.Tools.Testing.Update.Core.Update;
using Nemeio.Tools.Testing.Update.Core.Update.Tester.Settings;
using Newtonsoft.Json;

namespace Nemeio.Tools.Testing.Update.Application.Update.Tester
{
    public class TestSettingsParser : ITestSettingsParser
    {
        public TestSettings Parse(string data)
        {
            var settings = JsonConvert.DeserializeObject<TestSettingsDto>(data);
            var modelSettings = new TestSettings(
                new UpdateVersionRange(settings.VersionRange.Start, settings.VersionRange.End),
                settings.EnvironmentName,
                settings.ServerUri,
                settings.OutputPath
            );

            return modelSettings;
        }
    }
}
