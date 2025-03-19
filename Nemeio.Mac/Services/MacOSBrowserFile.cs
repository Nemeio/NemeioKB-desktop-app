using System.IO;
using AppKit;
using Foundation;

namespace Nemeio.Mac.Services
{
    public class MacOSBrowserFile : Nemeio.Core.Services.IBrowserFile
    {
        private NSWorkspace Workspace => NSWorkspace.SharedWorkspace;
        
        public string FindApplicationUrl()
        {
            var dialog = new NSOpenPanel();

            dialog.Title = "Choisir votre application ?";
            dialog.ShowsResizeIndicator = true;
            dialog.ShowsHiddenFiles = false;
            dialog.CanChooseDirectories = false;
            dialog.CanCreateDirectories = false;
            dialog.AllowsMultipleSelection = false;

            switch (dialog.RunModal(new string[] { "app" }))
            {
                case (long)NSModalResponse.OK:
                    return dialog.Url.Path;

                default:
                    return null;
            }
        }

        public void OpenApplication(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new FileNotFoundException("App not found !", path);
            }
            
            Workspace.LaunchApplication(path);
        }

        public void OpenApplications(string[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                OpenApplication(path[i]);
            }
        }

        public void OpenUrl(string url)
        {
            Workspace.OpenUrl(new NSUrl(url));
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
