using System.Globalization;
using Nemeio.Core.Services.AppSettings;

namespace Nemeio.Core.Test.Fakes
{
    public class FakeApplicationSettingsDbRepository : IApplicationSettingsDbRepository
    {
        public string CurrentCultureCode = "en-US";
        public ApplicationSettings FakeApplicationSettings;

        public bool ReadCalled { get; private set; }
        public bool SaveCalled { get; private set; }

        public FakeApplicationSettingsDbRepository()
        {
            FakeApplicationSettings = new ApplicationSettings(new CultureInfo(CurrentCultureCode), true, true, null, string.Empty);
        }

        public ApplicationSettings ReadApplicationSettings()
        {
            ReadCalled = true;

            return FakeApplicationSettings;
        }

        public void SaveApplicationSettings(ApplicationSettings applicationSettings)
        {
            SaveCalled = true;

            FakeApplicationSettings = applicationSettings;
        }
    }
}
