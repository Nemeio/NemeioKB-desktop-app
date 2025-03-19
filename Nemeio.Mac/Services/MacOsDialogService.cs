using System;
using AppKit;
using CoreFoundation;
using Microsoft.Extensions.Logging;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Enums;
using Nemeio.Core.Managers;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Resources;
using UserNotifications;

namespace Nemeio.Mac.Services
{
    public class MacOsDialogService : IDialogService
    {
        private readonly UNUserNotificationCenter _notificationCenter;
        private readonly ILanguageManager _languageManager;
        private readonly ILogger _logger;

        private nint NSALERT_FIRST_BUTTON_PRESSED = 1000;

        public MacOsDialogService(ILoggerFactory loggerFactory, ILanguageManager languageManager)
        {
            _logger = loggerFactory.CreateLogger<MacOsDialogService>();
            _languageManager = languageManager ?? throw new ArgumentNullException(nameof(languageManager));

            _notificationCenter = UNUserNotificationCenter.Current;
            _notificationCenter.Delegate = new MacOSNotificationDelegate();
        }

        public void ShowMessage(StringId title, StringId message)
        {
            var alertTitle = _languageManager.GetLocalizedValue(message);
            var alertMessage = _languageManager.GetLocalizedValue(StringId.CommonOk);

            DisplayAlert(alertTitle, alertMessage);
        }

        public void ShowMessageAsync(StringId title, StringId message)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                ShowMessage(title, message);
            });
        }

        public bool ShowYesNoQuestion(StringId title, StringId message)
        {
            var alertTitle = _languageManager.GetLocalizedValue(title);
            var alertMessage = _languageManager.GetLocalizedValue(message);

            return DisplayYesNoQuestion(alertTitle, alertMessage);
        }

        public void Show(string title, string message, bool modal = false, DialogType type = DialogType.None)
        {
            if (modal)
            {
                DisplayAlert(title, message);
            }
            else
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    DisplayAlert(title, message);
                });
            }
        }

        public bool ShowYesNoQuestion(string title, string message)
        {
            return DisplayYesNoQuestion(title, message);
        }

        private bool DisplayYesNoQuestion(string title, string message)
        {
            var alert = new NSAlert();
            alert.MessageText = title;
            alert.InformativeText = message;
            alert.AlertStyle = NSAlertStyle.Informational;
            alert.AddButton(_languageManager.GetLocalizedValue(StringId.CommonYes));
            alert.AddButton(_languageManager.GetLocalizedValue(StringId.CommonNo));

            return alert.RunModal() == NSALERT_FIRST_BUTTON_PRESSED;
        }

        private void DisplayAlert(string title, string message)
        {
            //  Title is not used on OSX
            //  But to be compliant with the interface, we need to implement it

            var alert = NSAlert.WithMessage(
                message,
                "OK",
                string.Empty,
                string.Empty,
                string.Empty
            );

            alert.RunModal();
        }

        public void ShowNotification(StringId title, StringId message)
        {
            var strTitle = _languageManager.GetLocalizedValue(title);
            var strMessage = _languageManager.GetLocalizedValue(message);

            var notificationContent = new UNMutableNotificationContent();
            notificationContent.Title = strTitle;
            notificationContent.Body = strMessage;
            notificationContent.Sound = UNNotificationSound.Default;

            _notificationCenter.GetNotificationSettings((settings) =>
            {
                if (settings.AuthorizationStatus == UNAuthorizationStatus.Authorized)
                {
                    //  Settings is ok
                    PublishNotification(notificationContent);
                }
                else
                {
                    var notificationFlag = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Provisional;

                    _notificationCenter.RequestAuthorization(notificationFlag, (granted, error) =>
                    {
                        if (error != null)
                        {
                            _logger.LogError(error.LocalizedDescription);

                            return;
                        }

                        if (granted)
                        {
                            PublishNotification(notificationContent);
                        }
                        else
                        {
                            _logger.LogWarning($"User denied notification access. Can't display notification with title <{title}> and body <{message}>");
                        }
                    });
                }
            });
        }

        //  WARNING! To publish notification, app need to be sign (also in debug)
        private void PublishNotification(UNMutableNotificationContent content)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var request = UNNotificationRequest.FromIdentifier(
                    Guid.NewGuid().ToString(),
                    content,
                    null   //  We want to send notification now
                );

                _notificationCenter.AddNotificationRequest(request, null);
            });
        }

        public void ShowMessageWithArgs(StringId title, StringId message, string[] args)
        {
            var alertTitle = _languageManager.GetLocalizedValue(title);
            var alertMessage = string.Format(_languageManager.GetLocalizedValue(message), args);
            Show(alertTitle, alertMessage, false);
        }
    }
}
