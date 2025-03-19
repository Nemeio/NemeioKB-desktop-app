using System.Collections.Generic;
using CefSharp;
using Nemeio.Wpf.Helpers.Cef.Dialog.Modals;

namespace Nemeio.Wpf.Helpers.Cef.Dialog
{
    internal sealed class CefSharpDialogHandler : IDialogHandler
    {
        public bool OnFileDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, CefFileDialogMode mode, CefFileDialogFlags flags, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
        {
            //  We manage any kind of modal except Save modal
            var isSaveModal = mode == CefFileDialogMode.Save;
            if (!isSaveModal)
            {
                //  This method is never called on MainThread by CefSharp
                //  We must dispatch on MainThread to display file entry selector
                App.Current.Dispatcher.Invoke(() =>
                {
                    var modal = FileSystemEntryModalFactory.CreateModal(mode, title, defaultFilePath, acceptFilters, selectedAcceptFilter, callback);
                    modal.Display();
                });

                //  We always want to display custom dialog
                return true;
            }

            return false;
        }
    }
}
