using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Managers;
using Nemeio.Core.PackageUpdater;
using Nemeio.Core.Services;
using Nemeio.Presentation.Modals;
using Nemeio.Presentation.PackageUpdater.UI;

namespace Nemeio.Mac.Modals
{
    public sealed class MacModalWindowFactory : IModalWindowFactory
    {
        private readonly ILanguageManager _languageManager;
        private readonly INemeioHttpService _httpService;
        private readonly IPackageUpdater _packageUpdater;
        private readonly IPackageUpdaterMessageFactory _messageFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IKeyboardController _keyboardController;
        private readonly IApplicationService _applicationService;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;

        public MacModalWindowFactory(ILoggerFactory loggerFactory, IApplicationService applicationService, IKeyboardController keyboardController, ILanguageManager languageManager, INemeioHttpService httpService, IPackageUpdater packageUpdater, IPackageUpdaterMessageFactory messageFactory, IActiveLayoutChangeHandler activeLayoutChangeHandler)
        {
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _packageUpdater = packageUpdater ?? throw new ArgumentNullException(nameof(packageUpdater));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
        }

        public IModalWindow CreateConfiguratorModal() => new MacConfiguratorModal(_httpService);

        public IModalWindow CreateFactoryResetModal() => new MacFactoryResetModal(_loggerFactory, _keyboardController, _languageManager, _applicationService);

        public IModalWindow CreateLanguageSelectionModal() => new MacLanguageSelectionModal(_languageManager);

        public IModalWindow CreateQuitModal() => new MacQuitModal(_languageManager, _activeLayoutChangeHandler, _keyboardController);

        public IModalWindow CreateUpdateModal() => new MacUpdateModal(_languageManager, _packageUpdater, _messageFactory);

        public IModalWindow CreateKeyboardInitErrorModal() => new MacKeyboardInitErrorModal(_languageManager);

        public IModalWindow CreateAdministratorModal(Core.Systems.Applications.Application application) => throw new NotSupportedException("Mac application not support administrator");

        public IModalWindow CreateRemovedByHidModal() => new MacRemovedByHidModal(_languageManager, _activeLayoutChangeHandler, _keyboardController);
    }
}
