using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Nemeio.Tools.LayoutConverter.Exceptions;
using Nemeio.Tools.LayoutConverter.Factories;
using Nemeio.Tools.LayoutConverter.Models;
using Nemeio.Tools.LayoutConverter.Providers;
using NUnit.Framework;

namespace Nemeio.Tools.LayoutConverter.Tests.Providers
{
    public class AugmentedImageFileProviderShould
    {
        [Test]
        public void AugmentedImageFileProvider_Constructor_NullParameter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new AugmentedImageFileProvider(null));
        }

        [TestCase("Classic")]
        [TestCase("Hide")]
        public void AugmentedImageFileProvider_GetFilesFrom_Ok(string folderName)
        {
            var testImagesFolderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "AzertySample",
                folderName
            );

            var imageType = new ImageTypeFactory().CreateImageType(folderName.ToLower());
            var augmentedImageFileProvider = new AugmentedImageFileProvider(imageType);
            augmentedImageFileProvider.LayoutId = "67896332";

            var files = augmentedImageFileProvider.GetFilesFrom(testImagesFolderPath);

            files.Count().Should().Be(imageType.SupportedModifiers.Count());
        }

        [Test]
        public void AugmentedImageFileProvider_GetFilesFrom_NullParameter_ThrowsArgumentNullException()
        {
            var imageType = new ImageType("fake", new List<string>() { Constantes.NoneModifier });
            var augmentedImageFileProvider = new AugmentedImageFileProvider(imageType);

            Assert.Throws<ArgumentNullException>(() => augmentedImageFileProvider.GetFilesFrom(null));
        }

        [Test]
        public void AugmentedImageFileProvider_GetFilesFrom_WithNotExistsDirectory_ThrowsToolException()
        {
            var imageType = new ImageType("fake", new List<string>() { Constantes.NoneModifier });
            var augmentedImageFileProvider = new AugmentedImageFileProvider(imageType);

            var exception = Assert.Throws<ToolException>(() => augmentedImageFileProvider.GetFilesFrom(@"Z:\this\folder\does\not\exists"));
            exception.ErrorCode.Should().Be(ToolErrorCode.DirectoryNotFound);
        }

        [Test]
        public void AugmentedImageFileProvider_GetFilesFrom_WithLessFileThanImageTypeCount_ThrowsToolException()
        {
            const string tempFolderName = "NemeioTestTmp";

            var folderPath = Path.Combine(
                Path.GetTempPath(),
                tempFolderName
            );

            Directory.CreateDirectory(folderPath);

            var imageType = new ImageType("fake", new List<string>() { Constantes.NoneModifier });
            var augmentedImageFileProvider = new AugmentedImageFileProvider(imageType);

            var exception = Assert.Throws<ToolException>(() => augmentedImageFileProvider.GetFilesFrom(folderPath));
            exception.ErrorCode.Should().Be(ToolErrorCode.InvalidDirectoryContent);

            Directory.Delete(folderPath, true);
        }

        [TestCase("Classic")]
        [TestCase("Hide")]
        public void AugmentedImageFileProvider_CheckEveryNeededFileArePresent_Ok(string folderName)
        {
            var testImagesFolderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Resources",
                "AzertySample",
                folderName
            );

            var imageType = new ImageTypeFactory().CreateImageType(folderName.ToLower());
            var augmentedImageFileProvider = new AugmentedImageFileProvider(imageType);
            augmentedImageFileProvider.LayoutId = "67896332";

            var result = augmentedImageFileProvider.CheckEveryNeededFileArePresent(testImagesFolderPath);

            result.Should().BeTrue();
        }

        [Test]
        public void AugmentedImageFileProvider_CheckEveryNeededFileArePresent_NullParameter_ThrowsArgumentNullException()
        {
            var imageType = new ImageType("fake", new List<string>() { Constantes.NoneModifier });
            var augmentedImageFileProvider = new AugmentedImageFileProvider(imageType);

            Assert.Throws<ArgumentNullException>(() => augmentedImageFileProvider.CheckEveryNeededFileArePresent(null));
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void AugmentedImageFileProvider_CheckEveryNeededFileArePresent_InvalidFolderPath_ThrowArgumentNullException(string invalidParameter)
        {
            var imageType = new ImageType("fake", new List<string>() { Constantes.NoneModifier });
            var augmentedImageFileProvider = new AugmentedImageFileProvider(imageType);

            Assert.Throws<ArgumentNullException>(() => augmentedImageFileProvider.CheckEveryNeededFileArePresent(invalidParameter));
        }
    }
}
