namespace Nemeio.Core.Services.AppSettings
{
    public interface IApplicationSettingsDbRepository
    {
        ApplicationSettings ReadApplicationSettings();

        void SaveApplicationSettings(ApplicationSettings applicationSettings);
    }
}
