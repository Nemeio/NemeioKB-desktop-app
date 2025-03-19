﻿using System;
using Nemeio.Core.Applications.Manifest;
using Nemeio.Core.Services;

namespace Nemeio.Core.PackageUpdater.Informations
{
    public class EmbeddedPackageInformation : PackageInformation
    {
        public string Filename { get; private set; }
        public string Path { get; set; }

        public EmbeddedPackageInformation(string filename, Version version)
            :base(version)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            Filename = filename;
        }

        public EmbeddedPackageInformation(FirmwareInformation firmwareInformation)
            : this(firmwareInformation.FileName, firmwareInformation.Version) { }

        public override string GetPackageFileName() => Filename;

        public override string FindPackageContainerFolderPath(IDocument document)
        {
            var folderPath = document.FirmwaresFolder;

            return folderPath;
        }
    }
}
