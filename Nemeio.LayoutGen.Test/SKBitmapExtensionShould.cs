using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nemeio.Core;
using Nemeio.LayoutGen.Extensions;
using NUnit.Framework;
using SkiaSharp;

namespace Nemeio.LayoutGen.Test
{
    [TestFixture]
    public class SKBitmapExtensionShould
    {
        private const byte color1 = 0, color2 = 64, color3 = 192, color4 = 255;

        private const string BlackEnglish = "black_67699721_English_United_States.png";
        private const string BlackFrench = "black_67896332_French_France.png";
        private const string BlackEmoji = "black-emoji.png";
        private const string WhiteEnglish = "white_67699721_English_United_States.png";
        private const string WhiteFrench = "white_67896332_French_France.png";
        private const string WhiteEmoji = "white-emoji.png";

        private SKBitmap bitmapGray;
        private SKBitmap bitmap4Colors;
        private SKBitmap small4Colors;

        [SetUp]
        public void SetUp()
        {
            int width = 16;
            int height = 16;
            var infos = new SKImageInfo(width, height, SKColorType.Gray8);
            bitmapGray = new SKBitmap(infos);
            byte intensity = 0;
            for (int y = 0; y < bitmapGray.Height; y++)
            {
                for (int x = 0; x < bitmapGray.Width; x++)
                {
                    bitmapGray.SetPixel(x, y, new SKColor(intensity, intensity, intensity));
                    intensity++;
                }
            }

            bitmap4Colors = new SKBitmap(infos);
            for (int y = 0; y < bitmapGray.Height; y++)
            {
                for (int x = 0; x < bitmapGray.Width; x++)
                {
                    intensity = 0;
                    if (x < bitmap4Colors.Width / 2)
                    {
                        intensity += 128;
                    }
                    if (y < bitmap4Colors.Height / 2)
                    {
                        intensity += 64;
                    }
                    bitmap4Colors.SetPixel(x, y, new SKColor(intensity, intensity, intensity));
                }
            }

            infos = new SKImageInfo(2, 2, SKColorType.Gray8);
            small4Colors = new SKBitmap(infos);
            small4Colors.SetPixel(0, 0, new SKColor(color1, color1, color1));
            small4Colors.SetPixel(1, 0, new SKColor(color3, color3, color3));
            small4Colors.SetPixel(0, 1, new SKColor(color2, color2, color2));
            small4Colors.SetPixel(1, 1, new SKColor(color4, color4, color4));
        }


        [TearDown]
        public void TearDown()
        {
            bitmapGray.Dispose();
            bitmap4Colors.Dispose();
            small4Colors.Dispose();
        }

        [Test]
        public void SKBitmapExtensions_01_Histogram_256Or4GrayLevel_WorksOk()
        {
            var histogram = bitmapGray.GetHistogram();
            Assert.IsTrue(histogram.Count == 256);

            histogram = bitmap4Colors.GetHistogram();
            Assert.IsTrue(histogram.Count == 4);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(8)]
        public void SKBitmapExtensions_02_ReduceToBitsPerPixel_GivenCount_WorksOk(int bitsPerPixel)
        {
            using (var newBitmap = bitmapGray.ReduceBitsPerPixel(bitsPerPixel))
            {
                var histogram = newBitmap.GetHistogram();
                int target = (1 << bitsPerPixel);
                Assert.That(histogram.Count == target);
            }
        }

        [Test]
        public void SKBitmapExtensions_03_DumpPNGImage_WorkOk()
        {
            var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.IsFalse(Directory.Exists(folderPath));

            using (var watcher = new DirectoryWatcher(folderPath))
            {
                var testPath = Path.Combine(folderPath, "test.png");
                Assert.False(File.Exists(testPath));

                bitmapGray.DumpPNGImage(testPath);
                Assert.True(File.Exists(testPath));

                var testInfo = new FileInfo(testPath);
                Assert.IsNotNull(testInfo);
                Assert.IsTrue(testInfo.Length > 0);
                Assert.IsTrue(bitmapGray.Bytes.Length > 0);
            }
            Assert.IsFalse(Directory.Exists(folderPath));
        }

        [Test]
        public void SKBitmapExtensions_04_ReadPNGImage_WorkOk()
        {
            var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.IsFalse(Directory.Exists(folderPath));

            using (var watcher = new DirectoryWatcher(folderPath))
            {
                var testPath = Path.Combine(folderPath, "test.png");
                bitmapGray.DumpPNGImage(testPath);

                var testInfo = new FileInfo(testPath);
                Assert.IsNotNull(testInfo);
                Assert.IsTrue(testInfo.Length > 0);

                using (var result = SKBitmapExtensions.ReadPNGImage(testPath))
                {
                    Assert.IsNotNull(result);
                    Assert.AreEqual(bitmapGray.Width, result.Width);
                    Assert.AreEqual(bitmapGray.Height, result.Height);
                    Assert.AreEqual(bitmapGray.Bytes, result.Bytes);
                }
            }
            Assert.IsFalse(Directory.Exists(folderPath));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(8)]
        public void SKBitmapExtensions_05_ConvertToBitsPerPixel_01_SmallBitmapGivenCount_WorksOk(int bitsPerPixel)
        {
            var newArray = small4Colors.ConvertToBitsPerPixel(bitsPerPixel);

            if (bitsPerPixel == 8)
            {
                Assert.IsTrue(newArray.Length == 4);
                Assert.IsTrue(newArray[0] == color4);
                Assert.IsTrue(newArray[1] == color2);
                Assert.IsTrue(newArray[2] == color3);
                Assert.IsTrue(newArray[3] == color1);
            }
            else if (bitsPerPixel == 4)
            {
                Assert.IsTrue(newArray.Length == 2);
                Assert.IsTrue(newArray[0] == 244);
                Assert.IsTrue(newArray[1] == 176);
            }
            else if (bitsPerPixel == 2)
            {
                Assert.IsTrue(newArray.Length == 1);
                Assert.IsTrue(newArray[0] == 216);
            }
            else if (bitsPerPixel == 1)
            {
                Assert.IsTrue(newArray.Length == 1);
                Assert.IsTrue(newArray[0] == 160);
            }
        }

        [Test]
        public void SKBitmapExtensions_05_ConvertTo1BitsPerPixel_02_SmallBitmapGivenCount_SimilarToLegacyConvertTo1Bpp()
        {
            var newArray = small4Colors.ConvertToBitsPerPixel(1);
            var checkArray = small4Colors.ConvertTo1Bpp();
            Assert.That(newArray, Is.EquivalentTo(checkArray));
        }

        [TestCase(BlackEnglish)]
        [TestCase(WhiteEnglish)]
        [TestCase(BlackFrench)]
        [TestCase(WhiteFrench)]
        [TestCase(BlackEmoji)]
        [TestCase(WhiteEmoji)]
        public void SKBitmapExtensions_05_ConvertTo1BitsPerPixel_03_TestInputFile_SimilarToLegacyConvertTo1Bpp(string bitmapResource)
        {
            // read source image
            using (var bitmap = FileHelper.ReadPNGFromResources(bitmapResource))
            {
                // test new algorithm
                var newArray = bitmap.ConvertToBitsPerPixel(1);

                // legacy algorithm but with enforced threshold at 127 instead of 140
                var oldArray = bitmap.ConvertTo1Bpp(seuilExterne: 127);

                Assert.IsTrue(oldArray.Length == newArray.Length);
                var indexSize = (int)Math.Ceiling(Math.Log10(newArray.Length));
                var diff = new List<int>();
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (newArray[i] != oldArray[i])
                    {
                        Console.WriteLine($"Difference {i.ToString().PadLeft(indexSize)}: new {newArray[i]}, old {oldArray[i]}");
                        diff.Add(i);
                    }
                }
                Assert.IsFalse(diff.Count > 0);
            }
        }

        // following test voluntarily commented. To be used only in regards to BLDLCK-2362 Study
        //[Test]
        public void BLDLCK_2362_StudyGrayLevels()
        {
            // build random temporary folder
            var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.IsFalse(Directory.Exists(folderPath));

            using (var watcher = new DirectoryWatcher(folderPath))
            {
                foreach (var bitmapResource in new List<string>() { BlackEnglish, WhiteEnglish, BlackFrench, WhiteFrench, BlackEmoji, WhiteEmoji })
                {
                    // read source image
                    using (var bitmap = FileHelper.ReadPNGFromResources(bitmapResource))
                    {
                        // analyze bitmapPath
                        var basename = Path.GetFileNameWithoutExtension(bitmapResource);

                        // full 256 colors
                        var histogram = bitmap.GetHistogram();
                        var nameParts = (new List<string>() { "histogram", basename, histogram.Count.ToString() }).ToArray();
                        var histoName = Path.Combine(folderPath, string.Join("_", nameParts) + ".txt");
                        DumpHistogram(histogram, histoName);

                        bitmap.DumpPNGImage(Path.Combine(folderPath, basename + "_256.png"));

                        int bitsPerPixel = 4;
                        while (bitsPerPixel > 0)
                        {
                            using (var reducedBitmap = bitmap.ReduceBitsPerPixel(bitsPerPixel))
                            {
                                // build histogram an dump
                                histogram = reducedBitmap.GetHistogram();
                                nameParts = (new List<string>() { "histogram", basename, histogram.Count.ToString() }).ToArray();
                                histoName = Path.Combine(folderPath, string.Join("_", nameParts) + ".txt");
                                DumpHistogram(histogram, histoName);

                                // build base output name
                                var filename = Path.Combine(folderPath, basename + "_" + $"{histogram.Count}");

                                // output to PNG for visual check
                                reducedBitmap.DumpPNGImage(filename + ".png");

                                // build compressed wallpaper
                                var array = reducedBitmap.CompressToBitsPerPixel(bitsPerPixel);
                                File.WriteAllBytes(filename + ".wallpaper", array);

                                bitsPerPixel /= 2;
                            }
                        }
                    }
                }
            
            }
            Assert.IsFalse(Directory.Exists(folderPath));
        }

        private void DumpHistogram(IDictionary<byte, int> histogram, string filename)
        {
            var valueFormat = $"D{Math.Ceiling(Math.Log10(histogram.Last().Key))}";
            var countFormat = $"D{Math.Ceiling(Math.Log10(histogram.Values.Max()))}";
            using (var file = new StreamWriter(filename))
            {
                foreach (var kvp in histogram)
                {
                    file.WriteLine($"{kvp.Key.ToString(valueFormat)}:{kvp.Value.ToString(countFormat)}");
                }
            }
        }
    }
}
