using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using SkiaSharp;

namespace Nemeio.LayoutGen.Extensions
{
    public static class SKBitmapExtensions
    {
        public static int ByteSize = 8;

        public static SKData ToPng(this SKBitmap bitmap)
        {
            using (var png = SKImage.FromBitmap(bitmap))
            {
                return png.Encode(SKEncodedImageFormat.Png, 100);
            }
        }

        public static SKBitmap Rotate(this SKBitmap bitmap, double angle)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            double radians = Math.PI * angle / 180;
            float sine = (float)Math.Abs(Math.Sin(radians));
            float cosine = (float)Math.Abs(Math.Cos(radians));
            int originalWidth = bitmap.Width;
            int originalHeight = bitmap.Height;
            int rotatedWidth = (int)(cosine * originalWidth + sine * originalHeight);
            int rotatedHeight = (int)(cosine * originalHeight + sine * originalWidth);

            var rotatedBitmap = new SKBitmap(rotatedWidth, rotatedHeight);

            using (var surface = new SKCanvas(rotatedBitmap))
            {
                surface.Translate(rotatedWidth / 2f, rotatedHeight / 2f);
                surface.RotateDegrees((float)angle);
                surface.Translate(originalWidth / 2f, originalHeight / 2f);
                surface.DrawBitmap(bitmap, new SKPoint());
            }
            return rotatedBitmap;
        }

        public static SKBitmap FlipHorizontal(this SKBitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            var flippedImage = new SKBitmap(bitmap.Width, bitmap.Height);

            using (var canvas = new SKCanvas(flippedImage))
            {
                canvas.Scale(-1, 1, flippedImage.Width / 2.0f, 0);
                canvas.DrawBitmap(bitmap, 0, 0);

                return flippedImage;
            }
        }

        /// <summary>
        /// The following method convert to black & white the input image, implicitly expected
        /// as gray Level image through RGB channels.
        /// Legacy implementation is a^plying an arbitrary threshold of 140 where 127 would have
        /// led better symmetry.
        /// The extra parameter threshold, unused most of time, is introduced to allow comparison tests
        /// with the new method ConvertToBitsPerPixel with bitserPixel set to 1. If this last method
        /// is going to be used, ConverTo1Bpp will become obsolete
        /// </summary>
        /// <param name="img">Bitmap being converted to 1 bit per pixel</param>
        /// <param name="seuilExterne">Threshold overload if 140 not desired</param>
        /// <returns>Bitmap thresholded and reduced byte array</returns>
        public static byte[] ConvertTo1Bpp(this SKBitmap img, int seuilExterne = -1)
        {
            const int bitPerPixel = 8;
            const uint mask = 0x80;
            int seuil = 140;
            if (seuilExterne > 0)
            {
                seuil = seuilExterne;
            }

            int width = img.Width;
            int height = img.Height;
            int index = width * height - 1;

            byte[] bytesArray = new byte[(width * height + bitPerPixel - 1) / bitPerPixel];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SKColor pixel = img.GetPixel(x, y);
                    int value = (pixel.Red + pixel.Green + pixel.Blue) / 3;

                    if (value > seuil)
                    {
                        bytesArray[index / bitPerPixel] |= (byte)(mask >> (index % bitPerPixel));
                    }
                    else
                    {
                        bytesArray[index / bitPerPixel] &= (byte)~(mask >> (index % bitPerPixel));
                    }

                    --index;
                }
            }

            return bytesArray;
        }

        /// <summary>
        /// Method to convert any Bitmap to a gray level one (usually RGBA 32 bits)
        /// </summary>
        /// <param name="bitmap">Input (self) bitmap to convert to gray</param>
        /// <returns>New bitmap reduced to gray level</returns>
        static public SKBitmap ConvertToGray(this SKBitmap bitmap)
        {
            // convert anyway to 8ppp to start with
            var infos = new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Gray8);
            var grayBitmap = new SKBitmap(infos);
            if (bitmap.CopyTo(grayBitmap))
            {
                return grayBitmap;
            }
            return null;
        }

        /// <summary>
        /// Method used to get a visual appreciation of bitsPerPixel encoding, by sub-sampling the
        /// final number of gray levels. No compress is realized here.
        /// </summary>
        /// <param name="bitmap">Gray level input bitmap</param>
        /// <param name="bitsPerPixel">Bits per pixel to determine number of shades available</param>
        /// <returns>Gray level image with sub sampling of gray levels</returns>
        public static SKBitmap ReduceBitsPerPixel(this SKBitmap bitmap, int bitsPerPixel)
        {
            var thresholder = BuildThresholdList(bitsPerPixel);
            var values = (1 << bitsPerPixel);
            var infos = new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Gray8);
            var newBitmap = new SKBitmap(infos);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    SKColor pixel = bitmap.GetPixel(x, y);
                    for (int i = 0; i < values; i++)
                    {
                        if (pixel.Red < thresholder[i].Key)
                        {
                            newBitmap.SetPixel(x, y, new SKColor(thresholder[i].Value, thresholder[i].Value, thresholder[i].Value));
                            break;
                        }
                    }
                }
            }
            return newBitmap;
        }

        /// <summary>
        /// Method equivalent to legacy ConvertTo1Bpp but allowing parameterization of the encoding size per
        /// pixel. This method assumes the input bitmap is gray level: hence we only focus on SKColor.Red
        /// component and reduced value is stored in the resulting array which is reduced according to the
        /// encoding (but not compressed yet).
        /// </summary>
        /// <param name="bitmap">Gray level input bitmap</param>
        /// <param name="bitsPerPixel">Desired encoding bits per pixel</param>
        /// <returns>Encoded image array (reversed order)</returns>
        public static byte[] ConvertToBitsPerPixel(this SKBitmap bitmap, int bitsPerPixel)
        {
            var thresholder = BuildThresholdList(bitsPerPixel, false);
            var values = (1 << bitsPerPixel);

            var arraySize = (int)Math.Ceiling((double)bitmap.Width * bitmap.Height * bitsPerPixel / ByteSize);
            var byteArray = new byte[arraySize];

            var index = bitmap.Width * bitmap.Height - 1;
            var reduceFactor = ByteSize / bitsPerPixel;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    SKColor pixel = bitmap.GetPixel(x, y);
                    var reducedIndex = index / reduceFactor;
                    var reducedShift = (index % reduceFactor) * bitsPerPixel;
                    for (int i = 0; i < values; i++)
                    {
                        if (pixel.Red < thresholder[i].Key)
                        {
                            var mask = thresholder[i].Value << (ByteSize - bitsPerPixel);
                            byteArray[reducedIndex] |= (byte)(mask >> reducedShift);
                            break;
                        }
                    }
                    --index;
                }
            }
            return byteArray;
        }

        /// <summary>
        /// Because SPI is on LSB.
        /// 
        /// Method equivalent to legacy ConvertTo1Bpp but allowing parameterization of the encoding size per
        /// pixel. This method assumes the input bitmap is gray level: hence we only focus on SKColor.Red
        /// component and reduced value is stored in the resulting array which is reduced according to the
        /// encoding (but not compressed yet).
        /// </summary>
        /// <param name="bitmap">Gray level input bitmap</param>
        /// <param name="bitsPerPixel">Desired encoding bits per pixel</param>
        /// <returns>Encoded image array (reversed order)</returns>
        public static byte[] ConvertTo2Bpp(this SKBitmap bitmap)
        {
            var bitsPerPixel = 2;
            var thresholder = new List<KeyValuePair<int, byte>>();
            thresholder.Add(new KeyValuePair<int, byte>(43, 0b00));
            thresholder.Add(new KeyValuePair<int, byte>(128, 0b10));
            thresholder.Add(new KeyValuePair<int, byte>(213, 0b01));
            thresholder.Add(new KeyValuePair<int, byte>(256, 0b11));

            var values = 1 << bitsPerPixel;

            var arraySize = (int)Math.Ceiling((double)bitmap.Width * bitmap.Height * bitsPerPixel / ByteSize);
            var byteArray = new byte[arraySize];

            var index = bitmap.Width * bitmap.Height - 1;
            var reduceFactor = ByteSize / bitsPerPixel;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    var reducedIndex = index / reduceFactor;
                    var reducedShift = index % reduceFactor * bitsPerPixel;

                    for (int i = 0; i < values; i++)
                    {
                        if (pixel.Red < thresholder[i].Key)
                        {
                            byteArray[reducedIndex] |= (byte)(thresholder[i].Value << reducedShift);
                            break;
                        }
                    }

                    --index;
                }
            }

            return byteArray;
        }

        /// <summary>
        /// Method to compute the histogram of the given Bitmap
        /// </summary>
        /// <param name="bitmap">Bitmap to be analyzed (self)</param>
        /// <returns>Associated histogram</returns>
        static public IDictionary<byte, int> GetHistogram(this SKBitmap bitmap)
        {
            int[] histogram = new int[256];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    SKColor pixel = bitmap.GetPixel(x, y);
                    histogram[pixel.Red]++;
                }
            }

            var result = new Dictionary<byte, int>();
            for (int i = 0; i < 256; i++)
            {
                if (histogram[i] > 0)
                {
                    result.Add((byte)i, histogram[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Method to dump a Bitmap to a PNG image file format
        /// </summary>
        /// <param name="grayBitmap">Bitmap</param>
        /// <param name="filename">Output file path</param>
        static public void DumpPNGImage(this SKBitmap grayBitmap, string filename)
        {
            using (var image = SKImage.FromBitmap(grayBitmap))
            {
                File.WriteAllBytes(filename, image.Encode(SKEncodedImageFormat.Png, 100).ToArray());
            }
        }

        /// <summary>
        /// Method to create a Bitmap from a PNG image file
        /// </summary>
        /// <param name="filename">Input file path</param>
        /// <returns>Resulting SkBitmap from file</returns>
        static public SKBitmap ReadPNGImage(string filename)
        {
            return SKBitmap.Decode(File.ReadAllBytes(filename));
        }

        /// <summary>
        /// This method first reduce bits per pixel encoding according to bitsPerPixel parameter
        /// and then compress the resulting byte array using GZip compression
        /// </summary>
        /// <param name="bitmap">Gray level bitmap to be compressed</param>
        /// <param name="bitsPerPixel">Desired encoding bits per pixel</param>
        /// <returns>Resulting GZipped byte array</returns>
        public static byte[] CompressToBitsPerPixel(this SKBitmap bitmap, int bitsPerPixel)
        {
            var convertByteData = bitmap.ConvertToBitsPerPixel(bitsPerPixel);
            return CompressByteArray(convertByteData);
        }

        /// <summary>
        /// This method is used to compress a byte array using GZip algorithm. Locally used so far
        /// but could have larger use out of SKBitmapExtensions.
        /// </summary>
        /// <param name="byteData">Byte array to be compressed</param>
        /// <returns>Resulting GZipped byte array</returns>
        static public byte[] CompressByteArray(byte[] byteData)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionLevel.Optimal))
            {
                zipStream.Write(byteData, 0, byteData.Length);
                zipStream.Close();

                return compressedStream.ToArray();
            }
        }

        /// <summary>
        /// Internal method used by various simplification methods: the goal here is to determine thresholds to be applied
        /// to gray level dynamic [0,255], to be symmetric as most as possible.
        /// </summary>
        /// <param name="bitsPerPixel">Desired encoding bits size</param>
        /// <param name="eightBitsEncode">Indicator on whether to reduce encoding</param>
        /// <returns>Resulting list of thresholds and replacement value</returns>
        internal static IList<KeyValuePair<int, byte>> BuildThresholdList(int bitsPerPixel, bool eightBitsEncode = true)
        {

            int values = (1 << bitsPerPixel);
            int numberOfThresholds = values - 1;
            float thresholdWidth = (float)255 / numberOfThresholds;
            float threshold = thresholdWidth / 2.0f;
            float intensity = 0.0f;

            List<KeyValuePair<int, byte>> thresholder = new List<KeyValuePair<int, byte>>();
            int curThreshold = (int)(threshold + 0.5f);
            byte curIntensity = (byte)(intensity + 0.5f);
            for (int i = 0; i < values - 1; i++)
            {
                thresholder.Add(new KeyValuePair<int, byte>(curThreshold, (curIntensity)));
                threshold += thresholdWidth;
                intensity += thresholdWidth;
                curThreshold = (int)(threshold + 0.5f);
                curIntensity = (byte)(intensity + 0.5f);
                if (!eightBitsEncode)
                {
                    curIntensity = (byte)(i + 1);
                }
            }
            // put an extra level as a sentinel
            curIntensity = 255;
            if (!eightBitsEncode)
            {
                curIntensity = (byte)(values - 1);
            }
            thresholder.Add(new KeyValuePair<int, byte>(256, curIntensity));

            return thresholder;
        }
    }
}
