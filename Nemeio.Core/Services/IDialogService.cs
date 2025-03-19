using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Enums;

namespace Nemeio.Core.Services
{
    public interface IDialogService
    {
        void ShowMessage(StringId title, StringId message);
        void ShowMessageWithArgs(StringId title, StringId message, string[] args);
        void ShowMessageAsync(StringId title, StringId msg);
        bool ShowYesNoQuestion(StringId title, StringId message);

        void Show(string title, string message, bool modal = false, DialogType type = DialogType.None);
        bool ShowYesNoQuestion(string title, string message);

        void ShowNotification(StringId title, StringId message);
    }
}
