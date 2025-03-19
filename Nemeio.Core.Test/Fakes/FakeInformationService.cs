using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Services;

namespace Nemeio.Core.Test.Fakes
{
    public class FakeInformationService : IInformationService
    {
        private PlatformArchitecture _currentPlatformArchitecture;
        private VersionProxy _currentVersionProxy;

        public bool GetApplicationArchitectureCalled { get; private set; }

        public bool GetApplicationVersionCalled { get; private set; }

        public void OverrideApplicationArchitecture(PlatformArchitecture architecture) => _currentPlatformArchitecture = architecture;

        public void OverrideApplicationVersion(VersionProxy version) => _currentVersionProxy = version;

        public PlatformArchitecture GetApplicationArchitecture()
        {
            GetApplicationArchitectureCalled = true;

            return _currentPlatformArchitecture;
        }

        public VersionProxy GetApplicationVersion()
        {
            GetApplicationVersionCalled = true;

            return _currentVersionProxy;
        }

        public PlatformArchitecture GetSystemArchitecture()
        {
            return Environment.Is64BitOperatingSystem ? PlatformArchitecture.x64 : PlatformArchitecture.x86 ;
        }

        public bool RunningOnAdministratorMode()
        {
            return false;
        }

        public Services.OperatingSystem GetOperatingSystem()
        {
            return Services.OperatingSystem.Windows;
        }

        public IEnumerable<string> GetSystemFonts()
        {
            return new List<string>() { "Arial" };
        }

        public Updater GetSoftwareUpdateVersion(UpdateModel updateModel)
        {
            var softwareUpdateModel = updateModel.Win.Softwares.First(x => x.Platform == GetApplicationArchitecture());

            return new Updater(
                softwareUpdateModel.Url,
                new VersionProxy(new Version(softwareUpdateModel.Version)),
                UpdateType.App,
                softwareUpdateModel.Checksum
            );
        }
    }
}
