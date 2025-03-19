using System.Windows.Input;
using MvvmCross.Platform;
using Nemeio.Core.Applications;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Wpf.Models;

namespace Nemeio.Wpf.ViewModel
{
    public class AskAdminRightViewModel : BaseViewModel
    {
        private readonly IApplicationSettingsProvider _applicationSettingsManager;
   
        private string _processName;
        private string _applicationTitle;
        private string _title;
        private string _informationText;
        private string _okButtonText;
        private string _cancelButtonText;
        private string _doNotAskAnymoreText;
        private bool _doNotAskAnymore;
        private ICommand _okCommand;
        private ICommand _cancelCommand;
        private bool _buttonsEnabled;

        public string ProcessName
        {
            get => _processName;
            set
            {
                _processName = value;
                InformationText = string.Format(_languageManager.GetLocalizedValue(StringId.AskAdminRightInformationText), _processName);
            }
        }

        public bool ButtonsEnabled
        {
            get => _buttonsEnabled;
            set
            {
                _buttonsEnabled = value;
                NotifyPropertyChanged(nameof(ButtonsEnabled));
            }
        }

        public string ApplicationTitle
        {
            get => _applicationTitle;
            set
            {
                _applicationTitle = value;
                NotifyPropertyChanged(nameof(ApplicationTitle));
            }
        }

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

        public string OkButtonText
        {
            get => _okButtonText;
            set
            {
                _okButtonText = value;
                NotifyPropertyChanged(nameof(OkButtonText));
            }
        }

        public string CancelButtonText
        {
            get => _cancelButtonText;
            set
            {
                _cancelButtonText = value;
                NotifyPropertyChanged(nameof(CancelButtonText));
            }
        }

        public string DoNotAskAnymoreText
        {
            get => _doNotAskAnymoreText;
            set
            {
                _doNotAskAnymoreText = value;
                NotifyPropertyChanged(nameof(DoNotAskAnymoreText));
            }
        }

        public bool DoNotAskAnymore
        {
            get => _doNotAskAnymore;
            set
            {
                _doNotAskAnymore = value;

                var showGrantPrivilegeWindow = !_doNotAskAnymore;

                if (_applicationSettingsManager.ShowGrantPrivilegeWindow != showGrantPrivilegeWindow)
                {
                    _applicationSettingsManager.ShowGrantPrivilegeWindow = showGrantPrivilegeWindow;
                }

                NotifyPropertyChanged(nameof(DoNotAskAnymore));
            }
        }

        public ICommand OkCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new CommandHandler(delegate
                    {
                        ButtonsEnabled = false;
                        if (System.Windows.Application.Current is App currentApplication)
                        {
                            currentApplication.RestartAsAdmin();
                        }
                        RequestClose();
                    }, () => true);
                }

                return _okCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new CommandHandler(delegate
                    {
                        ButtonsEnabled = false;
                        RequestClose();
                    }, () => true);
                }
                return _cancelCommand;
            }
        }

        public AskAdminRightViewModel()
        {
            _applicationSettingsManager = Mvx.Resolve<IApplicationSettingsProvider>();
            _doNotAskAnymore = !_applicationSettingsManager.ShowGrantPrivilegeWindow;

            ApplicationTitle = _languageManager.GetLocalizedValue(StringId.ApplicationTitleName);
            Title = _languageManager.GetLocalizedValue(StringId.AskAdminRightTitle);
            InformationText = _languageManager.GetLocalizedValue(StringId.AskAdminRightInformationText);
            OkButtonText = _languageManager.GetLocalizedValue(StringId.AskAdminRightOkButtonText);
            CancelButtonText = _languageManager.GetLocalizedValue(StringId.AskAdminRightCancelButtonText);
            DoNotAskAnymoreText = _languageManager.GetLocalizedValue(StringId.AskAdminRightDoNotAskAnymoreText);

            ButtonsEnabled = true;
        }
    }
}
