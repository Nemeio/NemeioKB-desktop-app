using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using OperatingSystem = Nemeio.Core.Services.OperatingSystem;

namespace Nemeio.Presentation
{
    public abstract class BaseInformationService : IInformationService
    {
        protected IFontProvider _fontProvider;

        public BaseInformationService(IFontProvider fontProvider)
        {
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
        }

        public abstract VersionProxy GetApplicationVersion();

        public abstract OperatingSystem GetOperatingSystem();

        public abstract bool RunningOnAdministratorMode();

        public abstract Updater GetSoftwareUpdateVersion(UpdateModel updateModel);

        public PlatformArchitecture GetApplicationArchitecture()
        {
            return Environment.Is64BitProcess ? PlatformArchitecture.x64 : PlatformArchitecture.x86;
        }

        public PlatformArchitecture GetSystemArchitecture()
        {
            return Environment.Is64BitOperatingSystem ? PlatformArchitecture.x64 : PlatformArchitecture.x86;
        }

        public IEnumerable<string> GetSystemFonts()
        {
            return _fontProvider.Fonts.Select(x => x.Name);
        }

        protected Updater GetSystemSoftwareUpdateVersion(SoftwareModel softwareModel)
        {
            var softwareUpdateModel = softwareModel.Softwares.First(x => x.Platform == GetApplicationArchitecture());

            return new Updater(
                softwareUpdateModel.Url,
                new VersionProxy(new Version(softwareUpdateModel.Version)),
                UpdateType.App,
                softwareUpdateModel.Checksum
            );
        }
    }
}
