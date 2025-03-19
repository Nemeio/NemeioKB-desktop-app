namespace Nemeio.Tools.Testing.Update.Application.Application
{
    public enum ApplicationExitCode
    {
        Success = 0,
        MissingAdministratorRight = -2,
        MissingSettingsFile = -3,
        FatalError = -99
    }
}
