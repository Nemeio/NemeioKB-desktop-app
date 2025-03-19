using System;
using System.IO;
using Nemeio.LayoutGen.Exceptions;
using Nemeio.LayoutGen.Models.Loader;

namespace Nemeio.LayoutGen.Factory
{
    internal class LGImageLoaderFactory
    {
        internal ILGImageLoader CreateImageLoader(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new InvalidOperationException($"<{filePath}> must not be null or whitespace");
            }

            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".svg": return new LGSvgLoader();
                case ".png": return new LGPngLoader();
                default:
                    throw new ImageFormatNotSupportedException(extension);
            }
        }
    }
}
