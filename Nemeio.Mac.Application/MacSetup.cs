using Microsoft.Extensions.Logging;
using MvvmCross.Core.ViewModels;
using MvvmCross.Mac.Platform;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using MvvmCross.Plugins.Messenger;
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
using Nemeio.Keyboard.Communication.Mac.Watchers;
using Nemeio.Keyboard.Communication.Tools.Utils;
using Nemeio.Mac.Application.Applications;
using Nemeio.Mac.Services;
using Nemeio.Platform.Hid.Mac.Keyboards;
using Nemeio.Platform.Mac.Applications;
using Nemeio.Platform.Mac.Keyboards;
using Nemeio.Platform.Mac.Layouts;
using Nemeio.Platform.Mac.Layouts.Systems;
using Nemeio.Platform.Mac.Sessions;
using Nemeio.Platform.Mac.Sleeps;
using Nemeio.Presentation;
using Nemeio.Mac.Application.Resources;
using Nemeio.Core.Tools.Watchers;
using Nemeio.Platform.Mac.Tools.Watchers;

namespace Nemeio.Mac.Application
{
    public class MacSetup : MvxMacSetup
    {
        private readonly ILoggerFactory _loggerFactory;

        public MacSetup(ILoggerFactory loggerFactory, IMvxApplicationDelegate applicationDelegate)
            : base(applicationDelegate)
        {
            _loggerFactory = loggerFactory;
        }

        protected override IMvxApplication CreateApp()
        {
            Mvx.RegisterSingleton(_loggerFactory);

            Mvx.RegisterType<IRetryHandler, RetryHandler>();
            Mvx.RegisterType<IWatcherFactory, MacWatcherFactory>();

            //  Layouts
            Mvx.LazyConstructAndRegisterSingleton<IKeyboardMapFactory, MacKeyboardMapFactory>();
            Mvx.LazyConstructAndRegisterSingleton<ILayoutNativeNameAdapter, MacLayoutNativeNameAdapter>();

            Mvx.LazyConstructAndRegisterSingleton<IResourceLoader, MacResourceLoader>();
            Mvx.LazyConstructAndRegisterSingleton<ILayoutImageGenerator, MacLayoutImageGenerator>();

            Mvx.ConstructAndRegisterSingleton<IKeyboardVersionParser, KeyboardVersionParser>();
            Mvx.ConstructAndRegisterSingleton<IKeyboardWatcherFactory, MacKeyboardWatcherFactory>();

            Mvx.LazyConstructAndRegisterSingleton<ISystemModifierDelegate, MacSystemModifierDelegate>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemHidAdapter, MacSystemHidAdapter>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemLayoutInteractor, MacSystemLayoutInteractor>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemActiveLayoutAdapter, MacSystemActiveLayoutAdapter>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemLayoutLoaderAdapter, MacSystemLayoutLoaderAdapter>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemForegroundApplicationAdapter, MacSystemForegroundApplicationAdapter>();
            Mvx.LazyConstructAndRegisterSingleton<ISystemSessionStateWatcher, MacSystemSessionStateWatcher>();
            Mvx.LazyConstructAndRegisterSingleton<ISleepModeAdapter, MacSleepModeAdapter>();

            Mvx.LazyConstructAndRegisterSingleton<IMvxMessenger, MvxMessengerHub>();
            Mvx.LazyConstructAndRegisterSingleton<IInformationService, MacInformationService>();
            Mvx.LazyConstructAndRegisterSingleton<IProtectedDataProvider, MacProtectedDataProvider>();

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
