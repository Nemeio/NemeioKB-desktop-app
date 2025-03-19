using System;
using System.Security.Principal;
using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Versions;
using Nemeio.Presentation;
using OperatingSystem = Nemeio.Core.Services.OperatingSystem;

namespace Nemeio.Windows.Application.Applications
{
    public class WinInformationService : BaseInformationService
    {
        private readonly IApplicationVersionProvider _applicationVersionProvider;

        public WinInformationService(IFontProvider fontProvider, IApplicationVersionProvider appVersionProvider) 
            : base(fontProvider) 
        {
            _applicationVersionProvider = appVersionProvider ?? throw new ArgumentNullException(nameof(appVersionProvider));
        }

        public override VersionProxy GetApplicationVersion()
        {
            var appVersion = _applicationVersionProvider.GetVersion();
            var strAppVersion = $"{appVersion.Major}.{appVersion.Minor}";

            return new VersionProxy(strAppVersion);
        }

        public override OperatingSystem GetOperatingSystem()
        {
            return OperatingSystem.Windows;
        }

        public override bool RunningOnAdministratorMode()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public override Updater GetSoftwareUpdateVersion(UpdateModel updateModel)
        {
            return GetSystemSoftwareUpdateVersion(updateModel.Win);
        }
    }
}
