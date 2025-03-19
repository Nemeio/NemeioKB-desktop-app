using System;
using System.IO;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Layouts.Images.AugmentedLayout
{
    public class AugmentedLayoutImageProvider : IAugmentedLayoutImageProvider
    {
        private const string ClassicTypeName = "classic";
        private const string HideTypeName = "hide";
        private const string GrayTypeName = "gray";
        private const string BoldTypeName = "bold";
        private const string WallpaperExtension = ".wallpaper.gz";
        private const string HidFolderName = "HID";

        private readonly IDocument _documentService;

        public AugmentedLayoutImageProvider(IDocument documentService)
        {
            _documentService = documentService;
        }

        public bool AugmentedLayoutImageExists(OsLayoutId layoutId, LayoutImageType imageType)
        {
            var filePath = GetAugmentedLayoutImagePath(layoutId, imageType);

            return File.Exists(filePath);
        }

        public bool AugmentedLayoutImageExists(ILayout layout)
        {
            var filePath = GetAugmentedLayoutImagePath(layout.LayoutInfo.OsLayoutId, layout.LayoutImageInfo.ImageType);

            return File.Exists(filePath);
        }

        public byte[] GetAugmentedLayoutImage(OsLayoutId layoutId, LayoutImageType imageType)
        {
            var filePath = GetAugmentedLayoutImagePath(layoutId, imageType);

            return File.ReadAllBytes(filePath);
        }

        private string GetAugmentedLayoutImagePath(OsLayoutId layoutId, LayoutImageType imageType)
        {
            return Path.Combine(
                GetAugmentedLayoutImageDirectoryPath(),
                $"{layoutId}_{GetImageTypeName(imageType)}{WallpaperExtension}"
            );
        }

        private string GetAugmentedLayoutImageDirectoryPath()
        {
            var nemeioAppDataPath = _documentService.UserNemeioFolder;

            return Path.Combine(nemeioAppDataPath, HidFolderName);
        }

        private string GetImageTypeName(LayoutImageType imageType)
        {
            switch (imageType)
            {
                case LayoutImageType.Classic:
                    return ClassicTypeName;
                case LayoutImageType.Gray:
                    return GrayTypeName;
                case LayoutImageType.Hide:
                    return HideTypeName;
                case LayoutImageType.Bold:
                    return BoldTypeName;
                default:
                    throw new InvalidOperationException("Unsupported value for LayoutImageType");
            }
        }
    }
}
