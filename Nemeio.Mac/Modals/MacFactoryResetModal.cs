using System;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.Mac.Windows.Alert.FactoryResetModal;

namespace Nemeio.Mac.Modals
{
    public sealed class MacFactoryResetModal : MacModalWindow<FactoryResetModalController>
    {
        private ILoggerFactory _loggerFactory;
        private IKeyboardController _keyboardController;
        private IApplicationService _applicationService;

        public MacFactoryResetModal(ILoggerFactory loggerFactory, IKeyboardController keyboardController, ILanguageManager languageManager, IApplicationService appService)
            : base(languageManager)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
            _applicationService = appService ?? throw new ArgumentNullException(nameof(appService));
        }

        public override FactoryResetModalController CreateNativeModal()
        {
            return FactoryResetModalController.Create(_loggerFactory, _languageManager, _keyboardController, _applicationService, () =>
            {
                OnClose();
            });
        }
    }
}
