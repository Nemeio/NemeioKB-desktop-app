using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Wpf.Models;

namespace Nemeio.Wpf.ViewModel
{
    public class SelectionLanguageViewModel : BaseViewModel
    {
        private string _title;
        private string _informationText;
        private string _validButtonText;
        private CultureInfo _selectedLanguage;
        private bool _validIsEnable;
        private IEnumerable<CultureInfo> _languages;
        private ICommand _selectLanguageCommand;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }

        public string InformationText
        {
            get => _informationText;
            set
            {
                _informationText = value;
                NotifyPropertyChanged(nameof(InformationText));
            }
        }

        public IEnumerable<CultureInfo> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                NotifyPropertyChanged(nameof(Languages));
            }
        }

        public CultureInfo SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                NotifyPropertyChanged(nameof(SelectedLanguage));
                ValidIsEnable = _selectedLanguage != null;
            }
        }

        public bool ValidIsEnable
        {
            get => _validIsEnable;
            set
            {
                _validIsEnable = value;
                NotifyPropertyChanged(nameof(ValidIsEnable));
            }
        }

        public string ValidButtonText
        {
            get => _validButtonText;
            set
            {
                _validButtonText = value;
                NotifyPropertyChanged(nameof(ValidButtonText));
            }
        }

        public ICommand SelectLanguageCommand
        {
            get
            {
                if (_selectLanguageCommand == null)
                {
                    _selectLanguageCommand = new CommandHandler(delegate
                    {
                        _languageManager.SelectMainLanguage(_selectedLanguage);
                        RequestClose();
                    }, () => true);
                }

                return _selectLanguageCommand;
            }
        }

        public SelectionLanguageViewModel()
        {
            Title = _languageManager.GetLocalizedValue(StringId.SelectLanguageTitle);
            InformationText = _languageManager.GetLocalizedValue(StringId.SelectLanguageInformationText);
            ValidButtonText = _languageManager.GetLocalizedValue(StringId.SelectLanguageValidButton);
            Languages = _languageManager.GetSupportedLanguages();
            SelectedLanguage = null;
        }
    }
}
