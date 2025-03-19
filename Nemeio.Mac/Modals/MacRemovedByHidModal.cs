using System;
using System.Threading.Tasks;
using AppKit;
using Nemeio.Core.Keyboard;
using Nemeio.Core.Layouts.Active;
using Nemeio.Core.Managers;
using Nemeio.Mac.Windows.Alert;

namespace Nemeio.Mac.Modals
{
    public class MacRemovedByHidModal : MacModalWindow<QuitViewController>
    {
        private readonly IActiveLayoutChangeHandler _activeLayoutChangeHandler;
        private readonly IKeyboardController _keyboardController;

        public MacRemovedByHidModal(ILanguageManager languageManager, IActiveLayoutChangeHandler activeLayoutChangeHandler, IKeyboardController keyboardController)
            : base(languageManager)
        {
            _activeLayoutChangeHandler = activeLayoutChangeHandler ?? throw new ArgumentNullException(nameof(activeLayoutChangeHandler));
            _keyboardController = keyboardController ?? throw new ArgumentNullException(nameof(keyboardController));
        }

        public override QuitViewController CreateNativeModal()
        {
            return QuitViewController.Create(_languageManager, async (result) =>
            {
                OnClose();

                if (result)
                {
                    var nemeio = _keyboardController.Nemeio;

                    await _activeLayoutChangeHandler.RequestApplicationShutdownAsync(nemeio);

                    NSApplication.SharedApplication.Terminate(null);
                }
            });
        }
    }
}
