using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Nemeio.UpdateInquiry.Builders;
using Nemeio.UpdateInquiry.Models;
using Nemeio.UpdateInquiry.Parser;

namespace Nemeio.UpdateInquiry.Repositories
{
    public class BlobStorageUpdateRepository : BaseStorageRepository, IUpdateRepository
    {
        private const string CpuBlobPrefix = "cpu-no-bootloader";
        private const string BleBlobPrefix = "ble";
        private const string IteBlobPrefix = "ite";
        private const string WinBlobPrefix = "win";
        private const string OsxBlobPrefix = "osx";
        private const string OsxMiniInstallerBlobPrefix = "osx-mini";
        private const string WinMiniInstallerBlobPrefix = "win-mini";
        private const string CliWindowsBlobPrefix = "cli-win";
        private const string CliLinuxBlobPrefix = "cli-linux";
        private const string WinImageLoaderBlobPrefix = "imageloader-win";
        private const string LinuxImageLoaderBlobPrefix = "imageloader-linux";
        private const string ImagePackageBuilderWinBlobPrefix = "imagepackagebuilder-win";
        private const string ImagePackageBuilderLinuxBlobPrefix = "imagepackagebuilder-linux";
        private const string PackageBlobPrefix = "package";
        private const string Package_zipBlobPrefix = "package_zip";
        private const string IsoMiniInstallerBlobPrefix = "iso";
        private const string SbsfuBlobPrefix = "sbsfu";
        private const string ScratchInstallerBlobPrefix = "scratch";
        private const string SfbBlobPrefix = "sfb";
        private const string NrfSecureBootloaderBlobPrefix = "nrf-secure-bootloader";
        private const string NrfSettingsBlobPrefix = "nrf-settings";
        private const string NrfSoftDeviceBlobPrefix = "nrf-soft-device";
        private const string NrfPackageAppOnlyBlobPrefix = "nrf-package-app-only";
        private const string NrfPackageCompleteBlobPrefix = "nrf-package-complete";
        private const string NrfScratchInstallerBlobPrefix = "nrf-scratch";

        public BlobStorageUpdateRepository(IConfigurationRoot configurationRoot)
            : base(configurationRoot) { }

        public Container GetContainerByEnvironment(UpdateEnvironment environment)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(
                new EnvironmentParser().Parse(environment)
            );

            var blobs = containerClient.GetBlobs(BlobTraits.All);
            var container = new Container();

            foreach (var blob in blobs)
            {
                var newBinary = new BinaryParser().Parse(blob, containerClient);

                if (newBinary != null)
                {
                    switch (newBinary.Component.ToLower())
                    {
                        case WinBlobPrefix:
                            container.Win.Add(newBinary);
                            break;
                        case CpuBlobPrefix:
                            container.Cpu.Add(newBinary);
                            break;
                        case BleBlobPrefix:
                            container.Ble.Add(newBinary);
                            break;
                        case IteBlobPrefix:
                            container.Ite.Add(newBinary);
                            break;
                        case OsxBlobPrefix:
                            container.Osx.Add(newBinary);
                            break;
                        case OsxMiniInstallerBlobPrefix:
                            container.OsxMiniInstaller.Add(newBinary);
                            break;
                        case WinMiniInstallerBlobPrefix:
                            container.WinMiniInstaller.Add(newBinary);
                            break;
                        case IsoMiniInstallerBlobPrefix:
                            container.IsoMiniInstaller.Add(newBinary);
                            break;
                        case SbsfuBlobPrefix:
                            container.Sbsfu.Add(newBinary);
                            break;
                        case ScratchInstallerBlobPrefix:
                            container.ScratchInstaller.Add(newBinary);
                            break;
                        case SfbBlobPrefix:
                            container.Sfb.Add(newBinary);
                            break;
                        case NrfSecureBootloaderBlobPrefix:
                            container.NrfSecureBootloader.Add(newBinary);
                            break;
                        case NrfSettingsBlobPrefix:
                            container.NrfSettings.Add(newBinary);
                            break;
                        case NrfSoftDeviceBlobPrefix:
                            container.NrfSoftDevice.Add(newBinary);
                            break;
                        case NrfPackageAppOnlyBlobPrefix:
                            container.NrfPackageAppOnly.Add(newBinary);
                            break;
                        case NrfPackageCompleteBlobPrefix:
                            container.NrfPackageComplete.Add(newBinary);
                            break;
                        case NrfScratchInstallerBlobPrefix:
                            container.NrfScratchInstaller.Add(newBinary);
                            break;
                        case CliWindowsBlobPrefix:
                            container.WinCli.Add(newBinary);
                            break;
                        case CliLinuxBlobPrefix:
                            container.LinuxCli.Add(newBinary);
                            break;
                        case WinImageLoaderBlobPrefix:
                            container.WinImageLoader.Add(newBinary);
                            break;
                        case LinuxImageLoaderBlobPrefix:
                            container.LinuxImageLoader.Add(newBinary);
                            break;
                        case ImagePackageBuilderWinBlobPrefix:
                            container.WinImagePackageBuilder.Add(newBinary);
                            break;
                        case ImagePackageBuilderLinuxBlobPrefix:
                            container.LinuxImagePackageBuilder.Add(newBinary);
                            break;
                        case PackageBlobPrefix:
                            container.Package.Add(newBinary);
                            break;
                        case Package_zipBlobPrefix:
                            container.Package_zip.Add(newBinary);
                            break;
                    }
                }
            }
            return container;
        }
    }
}
