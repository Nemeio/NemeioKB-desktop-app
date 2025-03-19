namespace Nemeio.Core.JsonModels
{
    public enum PlatformArchitecture
    {
        x64,
        x86
    }

    public class SoftwareVersionModel : VersionModel
    {
        public PlatformArchitecture Platform { get; set; }
    }
}
