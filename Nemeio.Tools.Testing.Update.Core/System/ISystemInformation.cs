using Nemeio.Core.JsonModels;
using Nemeio.Core.Services;

namespace Nemeio.Tools.Testing.Update.Core.System
{
    public interface ISystemInformation
    {
        OperatingSystem GetRunningSystem();
        PlatformArchitecture GetCurrentPlatform();
    }
}
