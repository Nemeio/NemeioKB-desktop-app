namespace Nemeio.Tools.Firmware.PackageBuilder.Applications
{
    internal enum ApplicationExitCode : int
    {
        Success = 0,
        UnknownFailure = -1,
        LoadManifestFailed = -2,
        ComposeUpdateFailed = -3,
        BuildPackageFailed = -4
    }
}
