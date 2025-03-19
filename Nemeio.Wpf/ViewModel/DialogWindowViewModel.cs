using System.Windows;
using Nemeio.Core.Enums;

namespace Nemeio.Wpf.ViewModel
{
    public class DialogWindowViewModel : BaseViewModel
    {
        public enum DialogResponse
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel
        }

        public string ImagePath
        {
            get
            {
                switch (Type)
                {
                    case DialogType.Asterisk:
                        return "/Images/Asterisk-48.png";
                    case DialogType.Exclamation: // fallthrough
                    case DialogType.Warning:
                        return "/Images/Exclamation-48.png";
                    case DialogType.Hand:
                        return "/Images/Hand-48.png";
                    case DialogType.Information:
                        return "/Images/Information-48.png";
                    case DialogType.Question:
                        return "/Images/Question-48.png";
                    case DialogType.Error: // fallthrough
                    case DialogType.Stop:
                        return "/Images/Stop-48.png";
                    default:
                        return "/Icons/nemeio.ico";
                }
            }
        }

        public string Title { get; }

        public string Body { get; }

        public DialogType Type { get; }

        public DialogResponse Buttons { get; }

        public string Yes
        {
            get
            {
                switch (Buttons)
                {
                    case DialogResponse.Ok: // fallthrough
                    case DialogResponse.OkCancel:
                        return _languageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.CommonOk).ToUpperInvariant();

                    case DialogResponse.YesNo: // fallthrough
                    case DialogResponse.YesNoCancel: // fallthrough
                    default:
                        return _languageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.CommonYes).ToUpperInvariant();
                }
            }
        }

        public string No
        {
            get
            {
                switch (Buttons)
                {
                    case DialogResponse.Ok: // fallthrough
                    case DialogResponse.OkCancel:
                        return "Error-NO";

                    case DialogResponse.YesNo: // fallthrough
                    case DialogResponse.YesNoCancel: // fallthrough
                    default:
                        return _languageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.CommonNo).ToUpperInvariant();
                }
            }
        }

        public string Cancel
        {
            get
            {
                switch (Buttons)
                {
                    case DialogResponse.Ok: // fallthrough
                    case DialogResponse.YesNo:
                        return "Error-CANCEL";

                    case DialogResponse.OkCancel: // fallthrough
                    case DialogResponse.YesNoCancel: // fallthrough
                    default:
                        return _languageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.CommonCancel).ToUpperInvariant();
                }
            }
        }

        public Visibility NoVisibility
        {
            get
            {
                switch (Buttons)
                {
                    case DialogResponse.Ok: // fallthrough
                    case DialogResponse.OkCancel:
                        return Visibility.Collapsed;

                    case DialogResponse.YesNo: // fallthrough
                    case DialogResponse.YesNoCancel: // fallthrough
                    default:
                        return Visibility.Visible;
                }
            }
        }

        public Visibility CancelVisibility
        {
            get
            {
                switch (Buttons)
                {
                    case DialogResponse.YesNoCancel: // fallthrough
                    case DialogResponse.OkCancel:
                        return Visibility.Visible;

                    case DialogResponse.Ok: // fallthrough
                    case DialogResponse.YesNo: // fallthrough
                    default:
                        return Visibility.Collapsed;
                }
            }
        }

        public DialogWindowViewModel(string title, string message, DialogType dialogType = DialogType.None, DialogResponse buttons = DialogResponse.Ok)
        {
            Type = dialogType;
            Title = title;
            Body = message;
            Buttons = buttons;
        }
    }
}
