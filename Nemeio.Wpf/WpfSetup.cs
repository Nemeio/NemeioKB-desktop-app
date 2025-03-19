using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Nemeio.Core.Icon;
using Nemeio.Core.Services;
using Nemeio.Core.Theme;
using Nemeio.Core.Versions;
using Nemeio.Presentation;
using Nemeio.Presentation.Menu;
using Nemeio.Presentation.Menu.Administrator;
using Nemeio.Presentation.Modals;
using Nemeio.Windows.Application;
using Nemeio.Wpf.Icon;
using Nemeio.Wpf.Menu.Administrator;
using Nemeio.Wpf.Modals;
using Nemeio.Wpf.Models;
using Nemeio.Wpf.Resolution;
using Nemeio.Wpf.Services;
using Nemeio.Wpf.Theme;
using Nemeio.Wpf.ViewModel;

namespace Nemeio.Wpf
{
    public class WpfSetup : WindowsSetup
    {
        public WpfSetup(Dispatcher uiThreadDispatcher, ILoggerFactory loggerFactory) 
            : base(uiThreadDispatcher, loggerFactory) { }

        protected override IMvxApplication CreateApp()
        {
            var application = base.CreateApp();

            //  Modals
            Mvx.LazyConstructAndRegisterSingleton<IModalWindowFactory, WinModalWindowFactory>();


            //  Resolution
            Mvx.LazyConstructAndRegisterSingleton<IScreenResolutionAdapter, WinScreenResolutionAdapter>();

            //  UI
            Mvx.LazyConstructAndRegisterSingleton<IAdministratorModalStrategyFactory, WinAdministratorModalStrategyFactory>();

            Mvx.RegisterType<IApplicationService, WinApplicationService>();
            Mvx.RegisterType<IDocument, WpfDocument>();
            Mvx.RegisterType<IBrowserFile, WpfBrowserFile>();
            Mvx.RegisterType<IDialogService, WpfDialogService>();
            Mvx.RegisterType<IApplicationVersionProvider, WinApplicationVersionProvider>();
            Mvx.RegisterType<IApplicationIconProvider, WinApplicationIconProvider>();
            Mvx.RegisterType<ISystemThemeProvider, WinSystemThemeProvider>();

            return application;
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Mvx.LazyConstructAndRegisterSingleton<IMainUserInterface, TaskBarIconMenuViewModel>();
        }
    }
}
