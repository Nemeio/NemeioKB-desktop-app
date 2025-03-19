using System;
using System.IO;
using System.Linq;
using Foundation;
using Nemeio.Core;
using Nemeio.Core.Services;

namespace Nemeio.Mac.Services
{
    public class MacOSDocument : IDocument
    {
        private const string EmbeddedResourcesFolderName = "Contents";
        private const string LibraryFolderName = "Library";
        private const string ApplicationSupportFolderName = "Application Support";

        private string ConfiguratorPath = $"{EmbeddedResourcesFolderName}/Configurator";
        private string FirmwaresFolderName = $"{EmbeddedResourcesFolderName}/Firmwares";

        public string DocumentPath
        {
            get
            {
                var paths = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User);
                var last = paths.Last();
                return last.Append(NemeioConstants.AppName, true).Path;
            }
        }

        public string GetConfiguratorPath()
        {
            var bundleUrl = NSBundle.MainBundle.BundleUrl.AbsoluteString;
            var configuratorPath = Path.Combine(bundleUrl, ConfiguratorPath);
            configuratorPath = configuratorPath.Remove(0, 7);

            return configuratorPath;
        }

        private NSUrl NemeioApplicationSupportPath
        {
            get
            {
                var paths = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User);
                var last = paths.Last();

                return last.Append(NemeioConstants.AppName, true);
            }
        }

        public string LogFolderPath => NemeioApplicationSupportPath.Append(NemeioConstants.LogFolderName, true).Path;

        public string TemporaryFolderPath => NemeioApplicationSupportPath.Append(NemeioConstants.TemporaryDirectoryName, true).Path;

        public string DatabasePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            LibraryFolderName,
            ApplicationSupportFolderName,
            NemeioConstants.AppName,
            NemeioConstants.DbFileName
        );

        public string UserNemeioFolder
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    LibraryFolderName,
                    ApplicationSupportFolderName,
                    NemeioConstants.AppName
                );
            }
        }

        public string FirmwaresFolder
        {
            get
            {
                var bundleUrl = NSBundle.MainBundle.BundleUrl.AbsoluteString;
                var firmwareFolderPath = Path.Combine(bundleUrl, FirmwaresFolderName);
                firmwareFolderPath = firmwareFolderPath.Remove(0, 7);

                return firmwareFolderPath;
            }
        }
    }
}
