using System;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.Core.Settings;
using Nemeio.Core.Systems.Applications;
using Nemeio.Presentation.Modals;

namespace Nemeio.Wpf.Modals
{
    public sealed class WinModalWindowFactory : IModalWindowFactory
    {
        private readonly INemeioHttpService _httpService;
        private readonly ILanguageManager _languageManager;
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;
        private readonly ISettingsHolder _settingsHolder;
        private readonly IKeyboardController _keyboardController;

        public WinModalWindowFactory(INemeioHttpService httpService, ILanguageManager languageManager, IActiveLayoutChangeHandler activeLayoutChangeHandler, ISettingsHolder settingsHolder, IKeyboardController keyboardController)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
            _settingsHolder = settingsHolder ?? throw new ArgumentNullException(nameof(settingsHolder));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
        }

        public IModalWindow CreateQuitModal() => new WinQuitModal(_activeLayoutChangeHandler, _keyboardController);
        public IModalWindow CreateUpdateModal() => new WinUpdateModal();
        public IModalWindow CreateLanguageSelectionModal() => new WinLanguageSelectionModal();
        public IModalWindow CreateConfiguratorModal() => new WinConfiguratorModal(_httpService, _languageManager, _settingsHolder);
        public IModalWindow CreateFactoryResetModal() => new WinFactoryResetModal();
        public IModalWindow CreateKeyboardInitErrorModal() => new WinKeyboardInitErrorModal();
        public IModalWindow CreateAdministratorModal(Application application) => new WinAdministratorModal(application.ProcessName);

        public IModalWindow CreateRemovedByHidModal() => new WinRemovedByHidModal(_activeLayoutChangeHandler, _keyboardController);
    }
}
