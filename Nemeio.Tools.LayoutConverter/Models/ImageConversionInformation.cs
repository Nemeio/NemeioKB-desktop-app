using System.Linq;
using Nemeio.Tools.LayoutConverter.Exceptions;
using Nemeio.Tools.LayoutConverter.Validators;

namespace Nemeio.Tools.LayoutConverter.Models
{
    internal class ImageConversionInformation
    {
        internal string LayoutId { get; private set; }
        internal string SelectedFolderPath { get; private set; } 
        internal string ImageFormat { get; private set; }
        internal ImageType ImageType { get; private set; }

        internal bool DebugMode { get; private set; }

        internal ImageConversionInformation(string layoutId, string selectedFolderPath, string imageFormat, ImageType imageType, bool debug = false)
        {
            if (string.IsNullOrWhiteSpace(layoutId))
            {
                throw new InvalidInputException(InputType.LayoutId, "The layout id is invalid.");
            }

            LayoutId = layoutId;

            if (string.IsNullOrWhiteSpace(selectedFolderPath))
            {
                throw new InvalidInputException(InputType.FolderPath, "The selected folder is invalid.");
            }

            SelectedFolderPath = selectedFolderPath;

            if (string.IsNullOrWhiteSpace(imageFormat))
            {
                var supportedFormats = ImageFormatValidator.SupportedFormats.Aggregate((x, y) => $"{x}, {y}");

                throw new InvalidInputException(InputType.ImageFormat, $"Current format is not supported. Please use expected format : {supportedFormats}");
            }

            ImageFormat = imageFormat;

            if (imageType == null)
            {
                throw new InvalidInputException(InputType.ImageType, "The source image type is unknow");
            }

            ImageType = imageType;
            DebugMode = debug;
        }
    }
}
