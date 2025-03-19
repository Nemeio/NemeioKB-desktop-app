using System;
using System.IO;
using System.Windows.Forms;
using Nemeio.Core;
using Nemeio.Core.Services;

namespace Nemeio.Wpf.Services
{
    public class WpfDocument : IDocument
    {
        private const string ConfiguratorFolder = "Configurator";
        private const string TempFolder = "tmp";
        private const string FirmwareFolderName = "Firmwares";

        public string DocumentPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), NemeioConstants.AppName);

        public string GetConfiguratorPath() => Path.Combine(Application.StartupPath, ConfiguratorFolder);

        public string LogFolderPath => Path.Combine(DocumentPath, NemeioConstants.LogFolderName);

        public string TemporaryFolderPath
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    NemeioConstants.AppName,
                    TempFolder
                );
            }
        }

        public string FirmwaresFolder => Path.Combine(Application.StartupPath, FirmwareFolderName);

        public string DatabasePath
        {
            get
            {
                if (!Directory.Exists(UserNemeioFolder))
                {
                    Directory.CreateDirectory(UserNemeioFolder);
                }

                return Path.Combine(
                    UserNemeioFolder,
                    NemeioConstants.DbFileName
                );
            }
        }

        public string UserNemeioFolder
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    NemeioConstants.AppName
                );
            }
        }
    }
}
