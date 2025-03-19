using System.IO;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models.Loader
{
    internal interface ILGImageLoader
    {
        SKBitmap LoadImage(string filePath, LGSize size);

        SKBitmap LoadImage(Stream stream, LGSize size);
    }
}
