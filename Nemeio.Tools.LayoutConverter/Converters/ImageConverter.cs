using System;
using System.Runtime.CompilerServices;
using Nemeio.LayoutGen.Extensions;
using Nemeio.Tools.LayoutConverter.Validators;

//  Needed for unit tests
[assembly: InternalsVisibleTo("Nemeio.Tools.LayoutConverter.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Nemeio.Tools.LayoutConverter.Converters
{
    internal class ImageConverter
    {
        private ImageFormat _imageFormat;
        private LayoutImageLoader _imageLoader;

        internal ImageConverter(ImageFormat imgFormat)
        {
            _imageFormat = imgFormat;
            _imageLoader = new LayoutImageLoader();
        }

        internal byte[] Convert(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            using (var bitmap = _imageLoader.LoadImage(filePath))
            {
                switch (_imageFormat)
                {
                    case ImageFormat.OneBitPerPixel:
                        return bitmap.ConvertTo1Bpp();
                    case ImageFormat.TwoBitsPerPixel:
                        return bitmap.ConvertToGray().ConvertTo2Bpp();
                    default:
                        throw new InvalidOperationException($"Unexpected image format <{_imageFormat}>");
                }
            } 
        }
    }
}
