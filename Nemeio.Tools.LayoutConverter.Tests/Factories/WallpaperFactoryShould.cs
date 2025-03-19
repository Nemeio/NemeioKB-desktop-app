using System;
using System.Collections.Generic;
using Nemeio.Tools.LayoutConverter.Factories;
using Nemeio.Tools.LayoutConverter.Models;
using Nemeio.Tools.LayoutConverter.Validators;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Factories
{
    public class WallpaperFactoryShould
    {
        [Test]
        public void WallpaperFactory_CreateWallpapper_WithNullFileList_ThrowsArgumentNullException()
        {
            var imageType = new ImageType("test", new List<string>() { "none" });
            var wallpaperFactory = new WallpaperFactory(ImageFormat.OneBitPerPixel);

            Assert.Throws<ArgumentNullException>(() => wallpaperFactory.CreateWallpapper(null, imageType));
        }

        [Test]
        public void WallpaperFactory_CreateWallpapper_WithEmptyFileList_ThrowsArgumentOutOfRangeException()
        {
            var imageType = new ImageType("test", new List<string>() { "none" });
            var wallpaperFactory = new WallpaperFactory(ImageFormat.OneBitPerPixel);

            Assert.Throws<ArgumentOutOfRangeException>(() => wallpaperFactory.CreateWallpapper(new List<string>(), imageType));
        }

        [Test]
        public void WallpaperFactory_CreateWallpapper_WithNullImageType_ThrowsArgumentNullException()
        {
            var wallpaperFactory = new WallpaperFactory(ImageFormat.OneBitPerPixel);
            var supportedModifiers = new List<string>() { @"C:\this\is\a\fake\file.png" };

            Assert.Throws<ArgumentNullException>(() => wallpaperFactory.CreateWallpapper(supportedModifiers, null));
        }
    }
}
