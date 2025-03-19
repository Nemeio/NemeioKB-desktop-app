using Microsoft.Extensions.Logging;
using MvvmCross.Core.ViewModels;
using MvvmCross.Mac.Platform;
using MvvmCross.Platform;
using Nemeio.Core.Icon;
using Nemeio.Core.Services;
using Nemeio.Core.Theme;
using Nemeio.Mac.Application;
using Nemeio.Mac.Icon;
using Nemeio.Mac.Menu.Administrator;
using Nemeio.Mac.Modals;
using Nemeio.Mac.Resolution;
using Nemeio.Mac.Services;
using Nemeio.Mac.StatusMenu;
using Nemeio.Mac.Theme;
using Nemeio.Presentation;
using Nemeio.Presentation.Menu;
using Nemeio.Presentation.Menu.Administrator;
using Nemeio.Presentation.Modals;

namespace Nemeio.Mac
{
    public class MacNemeioAppSetup : MacSetup
    {
        public MacNemeioAppSetup(ILoggerFactory loggerFactory, IMvxApplicationDelegate applicationDelegate)
            : base(loggerFactory, applicationDelegate) { }

        protected override IMvxApplication CreateApp()
        {
            var application = base.CreateApp();

            Mvx.LazyConstructAndRegisterSingleton<IApplicationService, MacOSApplicationService>();
            Mvx.LazyConstructAndRegisterSingleton<IDocument, MacOSDocument>();
            Mvx.LazyConstructAndRegisterSingleton<IBrowserFile, MacOSBrowserFile>();
            Mvx.LazyConstructAndRegisterSingleton<IDialogService, MacOsDialogService>();
            Mvx.LazyConstructAndRegisterSingleton<IApplicationIconProvider, MacApplicationIconProvider>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemThemeProvider, MacSystemThemeProvider>();

            //  Modals
            Mvx.LazyConstructAndRegisterSingleton<IModalWindowFactory, MacModalWindowFactory>();

            //  Administrator
            Mvx.LazyConstructAndRegisterSingleton<IAdministratorModalStrategyFactory, MacAdministratorModalStrategyFactory>();

            //  Resolution
            Mvx.LazyConstructAndRegisterSingleton<IScreenResolutionAdapter, MacScreenResolutionAdapter>();

            return application;
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Mvx.ConstructAndRegisterSingleton<IMainUserInterface, MacTaskBar>();
        }
    }
}
