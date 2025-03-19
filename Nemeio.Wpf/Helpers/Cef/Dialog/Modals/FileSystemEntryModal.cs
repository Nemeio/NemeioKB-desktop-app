using System.Collections.Generic;
using CefSharp;

namespace Nemeio.Wpf.Helpers.Cef.Dialog.Modals
{
    internal abstract class FileSystemEntryModal : IFileSystemEntryModal
    {
        protected readonly bool _canSelectMultiple = false;
        protected readonly string _title;
        protected readonly string _defaultFilePath;
        protected readonly List<string> _acceptFilters;
        protected readonly int _selectedAcceptFilter;
        private readonly IFileDialogCallback _callback;

        protected FileSystemEntryModal(bool canSelectMultiple, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
        {
            _canSelectMultiple = canSelectMultiple;
            _title = title;
            _defaultFilePath = defaultFilePath;
            _acceptFilters = acceptFilters;
            _selectedAcceptFilter = selectedAcceptFilter;
            _callback = callback;
        }

        /// <summary>
        /// The purpose of this method is to transform a URL into a string that no longer represents a URL.
        /// 
        /// In order to respect HTML5 and despite the custom file/folder selection modal, CefSharp applies a 
        /// conversion to prevent a Web application from knowing the real path.
        /// 
        /// However, in our case, it is imperative that we have access to the actual path. 
        /// So we replace the "\" by ">" and the ":\" by ">>". This method is not ideal (we would prefer to use WebSocket) 
        /// but it is the simplest today.
        /// </summary>
        protected string Transform(string url) => url.Replace(@":\", ">>").Replace(@"\", ">");

        protected void Finish(List<string> paths)
        {
            if (_callback.IsDisposed)
            {
                return;
            }

            _callback.Continue(_selectedAcceptFilter, paths);
        }

        protected void Cancel()
        {
            if (_callback.IsDisposed)
            {
                return;
            }

            _callback.Cancel();
        }

        public abstract void Display();
    }
}
