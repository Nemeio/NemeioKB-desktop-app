using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Nemeio.Tools.LayoutConverter.Exceptions;
using Nemeio.Tools.LayoutConverter.Factories;
using Nemeio.Tools.LayoutConverter.Models;
using Nemeio.Tools.LayoutConverter.Providers;
using Nemeio.Tools.LayoutConverter.Validators;

//  Needed for unit tests
[assembly: InternalsVisibleTo("Nemeio.Tools.LayoutConverter.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Nemeio.Tools.LayoutConverter.Converters
{
    internal class LayoutImageConverter
    {
        private const char InformationSeparator = '_';
        private const string ExportDirectoryName = "HID";

        private WallpaperFactory _wallpaper;
        private IAugmentedImageFileProvider _augmentedImageFileProvider;
        private ImageConversionInformation _imageConversionInformation;
        private IPathProvider _pathProvider;

        internal LayoutImageConverter(ImageConversionInformation imageConversionInformation, IAugmentedImageFileProvider augmentedImageFileProvider, IPathProvider pathProvider)
        {
            _imageConversionInformation = imageConversionInformation ?? throw new ArgumentNullException(nameof(imageConversionInformation));

            _augmentedImageFileProvider = augmentedImageFileProvider ?? throw new ArgumentNullException(nameof(augmentedImageFileProvider));
            _augmentedImageFileProvider.LayoutId = imageConversionInformation.LayoutId;

            var imageFormatValidator = new ImageFormatValidator();
            if (!imageFormatValidator.IsValidFormat(imageConversionInformation.ImageFormat))
            {
                throw new InvalidInputException(InputType.ImageFormat);
            }

            _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));

            _wallpaper = new WallpaperFactory(
                imageFormatValidator.GetFormatByName(_imageConversionInformation.ImageFormat), 
                _imageConversionInformation.DebugMode
            );
        }

        internal void CreateWallpaper()
        {
            if (_augmentedImageFileProvider == null)
            {
                throw new ArgumentNullException(nameof(_augmentedImageFileProvider));
            }

            IEnumerable<string> files = null;

            try
            {
                files = _augmentedImageFileProvider.GetFilesFrom(_imageConversionInformation.SelectedFolderPath);
            }
            catch (ToolException)
            {
                throw new InvalidInputException(InputType.FolderPath, "The selected folder is invalid.");
            }

            if (!_augmentedImageFileProvider.CheckEveryNeededFileArePresent(_imageConversionInformation.SelectedFolderPath))
            {
                throw new InvalidInputException(InputType.FolderContent, $"Specified folder not contains every needed images : none, shift, altgr, shift-altgr, function and capslock");
            }

            var imageType = _imageConversionInformation.ImageType;
            var layoutFiles = FilterFiles(files);
            var compressedImage = _wallpaper.CreateWallpapper(layoutFiles, imageType);

            Export(compressedImage);
        }

        private IEnumerable<string> FilterFiles(IEnumerable<string> files)
        {
            var layoutId = _imageConversionInformation.LayoutId;
            var type = _imageConversionInformation.ImageType;

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);

                var fileNameInfoParts = fileName.Split(InformationSeparator);
                var fileLayoutId = fileNameInfoParts.ElementAt(0);
                var fileModifier = fileNameInfoParts.ElementAt(1);
                var fileType = fileNameInfoParts.ElementAt(2);

                var isValidLayoutId = fileLayoutId.Equals(layoutId);
                var isValidImageType = fileType.Equals(type.TypeName);
                var isValidModifier = type.SupportedModifiers.Contains(fileModifier);

                if (isValidLayoutId && isValidImageType && isValidModifier)
                {
                    yield return file;
                }
            }
        }

        private void Export(byte[] compressedImage)
        {
            try
            {
                ExportToNemeioApplicationFolder(compressedImage);
            }
            catch (ToolException exception) when (exception.ErrorCode == ToolErrorCode.NemeioApplicationFolderNotFound)
            {
                //  Nemeio application seem to not be installed
                //  We save current wallpaper next to tool

                ExportNearToExecutable(compressedImage);

                throw;
            }
        }

        private void ExportToNemeioApplicationFolder(byte[] wallpaper)
        {
            var nemeioApplicationPath = _pathProvider.GetNemeioApplicationPath();

            if (!Directory.Exists(nemeioApplicationPath))
            {
                throw new ToolException(ToolErrorCode.NemeioApplicationFolderNotFound, "Nemeio application seem to not be installed on this computer. Your wallpaper has been saved near this application.");
            }

            var targetDirectoryPath = Path.Combine(nemeioApplicationPath, ExportDirectoryName);
            var targetDirectory = new DirectoryInfo(targetDirectoryPath);

            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            var exportFilePath = Path.Combine(targetDirectoryPath, GetExportFileName());
            
            File.WriteAllBytes(exportFilePath, wallpaper);
        }

        private void ExportNearToExecutable(byte[] compressedImage)
        {
            var exportPath = Path.Combine(Directory.GetCurrentDirectory(), GetExportFileName());

            File.WriteAllBytes(exportPath, compressedImage);
        }

        private string GetExportFileName() => $"{_imageConversionInformation.LayoutId}_{_imageConversionInformation.ImageType.TypeName}.wallpaper.gz";
    }
}
