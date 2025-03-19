namespace Nemeio.Tools.Image.ImagePackageBuilder.Applications
{
    internal enum ApplicationExitCode
    {
        Success = 0,
        UnknownFailure = -1,
        WritePackageFailed = -2,
        NotEnoughtInputFile = -3,
        InvalidJsonData = -4
    }
}
