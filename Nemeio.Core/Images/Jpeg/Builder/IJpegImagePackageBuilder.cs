using System.Collections.Generic;

namespace Nemeio.Core.Images.Jpeg.Builder
{
    public interface IJpegImagePackageBuilder
    {
        JpegImagePackage CreatePackage(IEnumerable<JpegImageData> files);
    }
}
