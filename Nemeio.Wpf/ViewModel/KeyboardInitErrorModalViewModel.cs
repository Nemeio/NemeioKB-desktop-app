using System.Windows.Input;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Wpf.Models;

namespace Nemeio.Wpf.ViewModel
{
    public class KeyboardInitErrorModalViewModel : BaseViewModel
    {
        private string _title;
        private string _explanation;
        private string _closeButtonText;
        private ICommand _closeCommand;

        #region Properties

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }

        public string Explanation
        {
            get => _explanation;
            set
            {
                _explanation = value;
                NotifyPropertyChanged(nameof(Explanation));
            }
        }


        public string CloseButtonText
        {
            get => _closeButtonText;
            set
            {
                _closeButtonText = value;
                NotifyPropertyChanged(nameof(CloseButtonText));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new CommandHandler(delegate
                    {
                        RequestClose();
                    }, () => true);
                }

                return _closeCommand;
            }
        }

        #endregion

        public KeyboardInitErrorModalViewModel()
        {
            Title = _languageManager.GetLocalizedValue(StringId.KeyboardInitFailedModalTitle);
            Explanation = _languageManager.GetLocalizedValue(StringId.KeyboardInitFailedModalExplanation);
            CloseButtonText = _languageManager.GetLocalizedValue(StringId.KeyboardInitFailedModalCloseButtonText);
        }
    }
}
