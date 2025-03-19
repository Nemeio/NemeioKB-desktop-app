namespace Nemeio.Core.Settings.Providers
{
    public interface ISettingsProvider
    {
        ISettings LoadSettings(string filePath);
    }
}
