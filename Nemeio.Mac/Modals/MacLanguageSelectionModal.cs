using Nemeio.Core.Managers;
using Nemeio.Mac.Windows.Alert.LanguageSelection;

namespace Nemeio.Mac.Modals
{
    public class MacLanguageSelectionModal : MacModalWindow<LanguageSelectionController>
    {
        public MacLanguageSelectionModal(ILanguageManager languageManager)
            : base(languageManager) { }

        public override LanguageSelectionController CreateNativeModal()
        {
            return LanguageSelectionController.Create(_languageManager, () =>
            {
                OnClose();
            });
        }
    }
}
