using System.Collections.Generic;
using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;

namespace Nemeio.Core.Services
{
    public enum OperatingSystem
    {
        Windows,
        Osx
    }

    public interface IInformationService
    {
        VersionProxy GetApplicationVersion();

        PlatformArchitecture GetApplicationArchitecture();

        PlatformArchitecture GetSystemArchitecture();

        bool RunningOnAdministratorMode();

        OperatingSystem GetOperatingSystem();

        IEnumerable<string> GetSystemFonts();

        Updater GetSoftwareUpdateVersion(UpdateModel updateModel);
    }
}
