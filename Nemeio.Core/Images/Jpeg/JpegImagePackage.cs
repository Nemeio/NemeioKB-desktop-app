using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nemeio.Core.Images.Jpeg
{
    public sealed class JpegImagePackage : IBinaryConvertible
    {
        public JpegImagePackageHeader Header { get; set; }
        public IEnumerable<JpegImage> Images { get; set; }

        public int ComputeSize()
        {
            return Header.ComputeSize() + Images
                .Select(image => image.Header.ComputeSize() + image.ComputeSize())
                .Aggregate((in1, in2) => in1 + in2);
        }

        public void Convert(BinaryWriter writer)
        {
            //  Refresh offset for each headers
            UpdateOffsets();

            //  We start with global header
            Header.Convert(writer);

            //  First we put all headers
            foreach (var image in Images)
            {
                image.Header.Convert(writer);
            }

            //  Then all images
            foreach (var image in Images)
            {
                image.Convert(writer);
            }
        }

        private void UpdateOffsets()
        {
            int standardSizeBeforeMe = Header.ComputeSize() + JpegImageHeader.HeaderSize * Images.Count();
            int firmwareSizeBeforeMe = 0;

            foreach (var image in Images)
            {
                image.Header.Offset = standardSizeBeforeMe + firmwareSizeBeforeMe;

                firmwareSizeBeforeMe += image.ComputeSize();
            }
        }
    }
}
