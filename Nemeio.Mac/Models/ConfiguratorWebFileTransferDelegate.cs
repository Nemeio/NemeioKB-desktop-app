using System;
using AppKit;
using Foundation;
using Nemeio.Core.DataModels.Locale;
using Nemeio.Core.Managers;
using WebKit;

namespace Nemeio.Mac.Models
{
    public class ConfiguratorWebFileTransferDelegate
    {
        private const string DataUrlFirstPart = "data:";
        private const int ModalResponseOk = 1;
        private const string NemeioFileExtension = "nemeio";

        private readonly ILanguageManager _languageManager;

        public ConfiguratorWebFileTransferDelegate(ILanguageManager languageManager)
        {
            _languageManager = languageManager;
        }

        public bool IsDownloadAction(WKNavigationAction action)
        {
            var isLinkActivated = action.NavigationType == WKNavigationType.LinkActivated;
            var isDataRequest = action.Request.Url.ToString().StartsWith(DataUrlFirstPart);

            return isDataRequest && isLinkActivated;
        }

        public void DownloadDataOverRequest(NSUrlRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var fileSelectorDialog = new NSSavePanel()
            {
                Title = _languageManager.GetLocalizedValue(StringId.FileSelectorTitle),
                ShowsResizeIndicator = true,
                ShowsHiddenFiles = false,
                Message = _languageManager.GetLocalizedValue(StringId.FileSelectorMessage),
                NameFieldStringValue = "",
                RequiredFileType = NemeioFileExtension
            };

            if (fileSelectorDialog.RunModal().Equals(ModalResponseOk))
            {
                var data = NSData.FromUrl(request.Url);
                var selectedPath = fileSelectorDialog.Url;

                data.Save(selectedPath, true);
            }
        }
    }
}
