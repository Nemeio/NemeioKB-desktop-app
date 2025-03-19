using System.Threading.Tasks;
using Nemeio.Core.Images.Jpeg;
using Nemeio.Tools.Core.Files;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Packages.Writer
{
    public interface IImagePackageWriter
    {
        Task WriteOnDiskAsync(JpegImagePackage package, string outputFilePath, LayoutFileDto layoutFile);
    }
}
