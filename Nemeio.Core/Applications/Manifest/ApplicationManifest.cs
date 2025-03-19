using System;
using System.IO;
using System.Threading.Tasks;
using Nemeio.Core.Services;

namespace Nemeio.Core.Applications.Manifest
{
    public class ApplicationManifest : IApplicationManifest
    {
        private const string ManifestFilename = "manifest.json";

        private readonly IDocument _document;
        private readonly IFirmwareManifestReader _firmwareManifestReader;

        public FirmwareManifest FirmwareManifest { get; private set; }

        public ApplicationManifest(IDocument document, IFirmwareManifestReader firmwareManifestReader)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _firmwareManifestReader = firmwareManifestReader ?? throw new ArgumentNullException(nameof(firmwareManifestReader));

            Task.Run(Initialize);
        }

        private async Task Initialize()
        {
            await LoadFirmwareManifestAsync();
        }

        private async Task LoadFirmwareManifestAsync()
        {
            var path = Path.Combine(_document.FirmwaresFolder, ManifestFilename);
            var manifest = await _firmwareManifestReader.ReadAsync(path);

            FirmwareManifest = manifest;
        }
    }
}
