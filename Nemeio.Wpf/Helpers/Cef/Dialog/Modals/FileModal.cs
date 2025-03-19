using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CefSharp;

namespace Nemeio.Wpf.Helpers.Cef.Dialog.Modals
{
    internal sealed class FileModal : FileSystemEntryModal
    {
        public FileModal(bool canSelectMultiple, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
            : base(canSelectMultiple, title, defaultFilePath, acceptFilters, selectedAcceptFilter, callback) { }

        public override void Display()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = _defaultFilePath;
                openFileDialog.Filter = ComposeFilters(_acceptFilters);
                openFileDialog.FilterIndex = _selectedAcceptFilter;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = _title;
                openFileDialog.Multiselect = _canSelectMultiple;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePaths = openFileDialog.FileNames.Select(x => Transform(x)).ToList();
                    var pathList = new List<string>(filePaths);

                    Finish(pathList);
                }
                else
                {
                    Cancel();
                }
            }
        }

        private string ComposeFilters(List<string> filters)
        {
            const string defaultValue = "All files (*.*)|*.*";

            if (filters == null)
            {
                return defaultValue;
            }

            if (!filters.Any())
            {
                return defaultValue;
            }
            else
            {
                if (filters.Count == 1)
                {
                    return ComposeFilter(filters.First());
                }
                else
                {
                    var result = filters.Aggregate((x, y) => $"{ComposeFilter(x)}|{ComposeFilter(y)}");

                    return result;
                }
            }
        }

        private string ComposeFilter(string filter) => $"Fichiers {filter} (*{filter})|*{filter}";
    }
}
