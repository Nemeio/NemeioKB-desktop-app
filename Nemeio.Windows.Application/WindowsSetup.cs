using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.Wpf.Platform;
using MvvmCross.Wpf.Views.Presenters;
using Nemeio.Core.Keyboard.Communication.Utils;
using Nemeio.Core.Keyboard.Communication.Watchers;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Name;
using Nemeio.Core.Resources;
using Nemeio.Core.Services;
using Nemeio.Core.Systems;
using Nemeio.Core.Systems.Applications;
using Nemeio.Core.Systems.Hid;
using Nemeio.Core.Systems.Sessions;
using Nemeio.Core.Systems.Sleep;
using Nemeio.Core.Tools.Retry;
using Nemeio.Core.Tools.Watchers;
using Nemeio.Keyboard.Communication.Tools.Utils;
using Nemeio.Keyboard.Communication.Windows.Watchers;
using Nemeio.Platform.Hid.Windows.Keyboards;
using Nemeio.Platform.Windows;
using Nemeio.Platform.Windows.Applications;
using Nemeio.Platform.Windows.Layouts;
using Nemeio.Platform.Windows.Layouts.Images;
using Nemeio.Platform.Windows.Layouts.Systems;
using Nemeio.Platform.Windows.Sessions;
using Nemeio.Platform.Windows.Sleep;
using Nemeio.Presentation;
using Nemeio.Platform.Windows.Tools.Watchers;
using Nemeio.Windows.Application.Applications;
using Nemeio.Windows.Application.Resources;

namespace Nemeio.Windows.Application
{
    public abstract class WindowsSetup : MvxWpfSetup
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        private class MyPresenter : MvxWpfViewPresenter { }

        public WindowsSetup(Dispatcher uiThreadDispatcher, ILoggerFactory loggerFactory)
            : base(uiThreadDispatcher, new MyPresenter())
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<WindowsSetup>();
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();

            _logger.LogInformation($"WindowsSetup.InitializeIoC");

            Mvx.RegisterSingleton(_loggerFactory);
        }

        protected override IMvxApplication CreateApp()
        {
            _logger.LogInformation($"WindowsSetup.CreateApp");

            Mvx.RegisterType<WinOsLayoutIdBuilder, WinOsLayoutIdBuilder>();
            Mvx.RegisterType<IRetryHandler, RetryHandler>();
            Mvx.RegisterType<IWatcherFactory, WinWatcherFactory>();

            //  Layout
            Mvx.LazyConstructAndRegisterSingleton<IResourceLoader, WinResourceLoader>();
            Mvx.LazyConstructAndRegisterSingleton<IKeyboardMapFactory, WinKeyboardMapFactory>();
            Mvx.LazyConstructAndRegisterSingleton<ILayoutNativeNameAdapter, WinLayoutNativeNameAdapter>();

            //  System
            Mvx.LazyConstructAndRegisterSingleton<ISystemLayoutInteractor, WinSystemLayoutInteractor>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemActiveLayoutAdapter, WinSystemActiveLayoutAdapter>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemLayoutLoaderAdapter, WinSystemLayoutLoaderAdapter>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemForegroundApplicationAdapter, WinSystemForegroundApplicationAdapter>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemModifierDelegate, WinSystemModifierDelegate>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemHidAdapter, WinSystemHidAdapter>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemSessionStateWatcher, WinSystemSessionStateWatcher>();
            Mvx.LazyConstructAndRegisterSingleton<ISleepModeAdapter, WinSleepModeAdapter>();

            //  Keyboard
            Mvx.ConstructAndRegisterSingleton<IKeyboardVersionParser, KeyboardVersionParser>();
            Mvx.ConstructAndRegisterSingleton<IKeyboardWatcherFactory, WinKeyboardWatcherFactory>();

            Mvx.RegisterType<ILayoutImageGenerator, WinLayoutImageGenerator>();
            Mvx.RegisterType<IProtectedDataProvider, WinProtectedDataProvider>();
            Mvx.RegisterType<IInformationService, WinInformationService>();

            return new Presentation.Application();
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            Presentation.Application.InitializeBeforeLastChance();
        }

        protected override IMvxTrace CreateDebugTrace() => new MvxTraceProxy(_loggerFactory);
    }
}
