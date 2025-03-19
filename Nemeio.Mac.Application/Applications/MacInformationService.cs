using Foundation;
using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.Presentation;

namespace Nemeio.Mac.Application.Applications
{
    public class MacInformationService : BaseInformationService
    {
        public MacInformationService(IFontProvider fontProvider) 
            : base(fontProvider) { }

        public override VersionProxy GetApplicationVersion()
        {
            var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();

            return new VersionProxy(version);
        }

        public override OperatingSystem GetOperatingSystem()
        {
            return OperatingSystem.Osx;
        }

        public override bool RunningOnAdministratorMode()
        {
            return false;
        }

        public override Updater GetSoftwareUpdateVersion(UpdateModel updateModel)
        {
            return GetSystemSoftwareUpdateVersion(updateModel.Osx);
        }
    }
}
