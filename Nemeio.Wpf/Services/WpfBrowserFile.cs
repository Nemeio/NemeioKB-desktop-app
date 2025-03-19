using System;
using System.Diagnostics;
using System.IO;

namespace Nemeio.Wpf.Services
{
    public class WpfBrowserFile : Nemeio.Core.Services.IBrowserFile
    {
        public string FindApplicationUrl()
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.Multiselect = false;
            openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms);
            openDialog.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
            openDialog.FilterIndex = 1;

            return openDialog.ShowDialog() == true ? openDialog.FileName : null;
        }

        public void OpenApplications(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                OpenApplication(paths[i]);
            }
        }

        public void OpenApplication(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Application not found", path);
            }

            Process.Start(path);
        }

        public void OpenUrl(string url)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(url);
            Process.Start(sInfo);
        }

        public void OpenUrls(string[] urls)
        {
            for (int i = 0; i < urls.Length; i++)
            {
                OpenUrl(urls[i]);
            }
        }
    }
}
