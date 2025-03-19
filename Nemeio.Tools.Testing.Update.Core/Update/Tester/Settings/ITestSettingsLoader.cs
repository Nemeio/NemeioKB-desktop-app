namespace Nemeio.Tools.Testing.Update.Core.Update.Tester.Settings
{
    public interface ITestSettingsLoader
    {
        TestSettings LoadSettings(string path);
    }
}
