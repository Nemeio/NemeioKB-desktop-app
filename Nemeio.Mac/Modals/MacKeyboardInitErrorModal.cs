using System;
using Nemeio.Core.Managers;
using Nemeio.Mac.Windows.Alert.KeyboardInitErrorModal;

namespace Nemeio.Mac.Modals
{
    public class MacKeyboardInitErrorModal : MacModalWindow<KeyboardInitErrorModalController>
    {
        public MacKeyboardInitErrorModal(ILanguageManager languageManager)
            : base(languageManager) { }

        public override KeyboardInitErrorModalController CreateNativeModal()
        {
            return KeyboardInitErrorModalController.Create(_languageManager, () =>
            {
                OnClose();
            });
        }
    }
}
