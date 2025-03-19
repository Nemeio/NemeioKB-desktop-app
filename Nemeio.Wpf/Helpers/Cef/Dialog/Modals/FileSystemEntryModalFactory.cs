using System;
using System.Collections.Generic;
using CefSharp;

namespace Nemeio.Wpf.Helpers.Cef.Dialog.Modals
{
    internal static class FileSystemEntryModalFactory
    {
        public static IFileSystemEntryModal CreateModal(CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
        {
            switch (mode)
            {
                case CefFileDialogMode.Open:
                case CefFileDialogMode.OpenMultiple:
                    return new FileModal(mode == CefFileDialogMode.OpenMultiple, title, defaultFilePath, acceptFilters, selectedAcceptFilter, callback);
                case CefFileDialogMode.OpenFolder:
                    return new FolderModal(title, defaultFilePath, acceptFilters, selectedAcceptFilter, callback);
                default:
                    throw new InvalidOperationException($"Mode <{mode}> not supported");
            }
        }
    }
}
