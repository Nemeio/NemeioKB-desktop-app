using System;
using System.Windows;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Enums;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.Wpf.UserControls;
using Nemeio.Wpf.ViewModel;

namespace Nemeio.Wpf.Services
{
    public class WpfDialogService : IDialogService
    {
        private readonly ILanguageManager _languageManager;

        public WpfDialogService(ILanguageManager languageManager)
        {
            _languageManager = languageManager;
        }

        public void ShowMessage(StringId title, StringId message)
        {
            var alertTitle = _languageManager.GetLocalizedValue(title);
            var alertMessage = _languageManager.GetLocalizedValue(message);

            Show(alertTitle, alertMessage, modal: true);
        }

        public void ShowMessageAsync(StringId title, StringId msg)
        {
            var alertTitle = _languageManager.GetLocalizedValue(title);
            var alertMessage = _languageManager.GetLocalizedValue(msg);

            Show(alertTitle, alertMessage, modal: false);
        }

        public bool ShowYesNoQuestion(StringId title, StringId message)
        {
            var alertTitle = _languageManager.GetLocalizedValue(title);
            var alertMessage = _languageManager.GetLocalizedValue(message);

            return ShowYesNoQuestion(alertTitle, alertMessage);
        }

        public void Show(string title, string message, bool modal = false, DialogType type = DialogType.None)
        {
            DialogShow(title, message, modal, type);
        }

        public bool ShowYesNoQuestion(string title, string message)
        {
            var result = DialogShow(title, message, true, DialogType.Question);
            if (!result.HasValue)
            {
                throw new InvalidOperationException("WpfDialogService. ShowYesNoQuestion response cannot be null");
            }
            return result.Value;
        }

        private bool? DialogShow(string title, string message, bool modal = false, DialogType type = DialogType.None)
        {
            bool? result = null;
            Application.Current.Dispatcher.Invoke(delegate
            {
                DialogWindow dialog = new DialogWindow(modal);
                DialogWindowViewModel viewModel = new DialogWindowViewModel(title, message, type);
                dialog.DataContext = viewModel;
                if (modal)
                {
                    dialog.ShowDialog();
                    result = dialog.DialogResult;
                }
                else
                {
                    dialog.Show();
                }
            });
            return result;
        }

        public void ShowNotification(StringId title, StringId message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var toolbarIcon = Application.Current.MainWindow as TaskBarIconView;
                if (toolbarIcon == null)
                {
                    return;
                }

                var strTitle = _languageManager.GetLocalizedValue(title);
                var strMessage = _languageManager.GetLocalizedValue(message);

                toolbarIcon.TaskBarIconInstance.ShowBalloonTip(strTitle, strMessage, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.None);
            });
        }

        public void ShowMessageWithArgs(StringId title, StringId message, string[] args)
        {
            var alertTitle = _languageManager.GetLocalizedValue(title);
            var alertMessage = string.Format(_languageManager.GetLocalizedValue(message), args);
            Show(alertTitle, alertMessage, modal: true);
        }
    }
}
