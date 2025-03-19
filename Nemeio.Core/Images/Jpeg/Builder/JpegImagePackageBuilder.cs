using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Extensions;

namespace Nemeio.Core.Images.Jpeg.Builder
{
    public sealed class JpegImagePackageBuilder : IJpegImagePackageBuilder
    {
        private const int FormatVersion = 2;
        private static readonly uint PackageTag = 0x1d1c1d1c;

        public JpegImagePackage CreatePackage(IEnumerable<JpegImageData> files)
        {
            if (files == null)
            {
                throw new ArgumentNullException(nameof(files));
            }

            if (!files.Any())
            {
                throw new ArgumentNullException(nameof(files), "List must contains at least one file");
            }

            //  Load images
            var images = new List<JpegImage>();

            foreach (var imageFile in files)
            {
                var header = new JpegImageHeader(
                    imageFile.CompressionType, 
                    offset: 0,
                    size: imageFile.Content.Length
                );

                var image = new JpegImage(header, imageFile.Content);

                images.Add(image);
            }

            var imagePackageHeader = new JpegImagePackageHeader()
            {
                Signature = ComputeSignature(),
                Tag = PackageTag,
                FormatVersion = FormatVersion,
                FirmwareCount = images.Count.ToByte()
            };

            //  Create image package
            var imagePackage = new JpegImagePackage()
            {
                Header = imagePackageHeader,
                Images = images
            };

            imagePackage.Header.Size = imagePackage.ComputeSize();

            return imagePackage;
        }

        private byte[] ComputeSignature()
        {
            // FIXME [KSB]: Must be done on BLDLCK-2817.
            return new byte[32];
        }
    }
}
