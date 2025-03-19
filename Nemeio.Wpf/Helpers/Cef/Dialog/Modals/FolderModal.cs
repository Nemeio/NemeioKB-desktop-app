using System.Collections.Generic;
using System.Windows.Forms;
using CefSharp;

namespace Nemeio.Wpf.Helpers.Cef.Dialog.Modals
{
    internal sealed class FolderModal : FileSystemEntryModal
    {
        public FolderModal(string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
            : base(canSelectMultiple: false, title, defaultFilePath, acceptFilters, selectedAcceptFilter, callback) { }

        public override void Display()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.SelectedPath = _defaultFilePath;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    var selectedFolderPath = Transform(folderDialog.SelectedPath);
                    var paths = new List<string>() { selectedFolderPath };

                    Finish(paths);
                }
                else
                {
                    Cancel();
                }
            }
        }
    }
}
