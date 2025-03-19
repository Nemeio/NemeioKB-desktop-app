using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nemeio.Tools.LayoutConverter.Exceptions;
using Nemeio.Tools.LayoutConverter.Models;

namespace Nemeio.Tools.LayoutConverter.Providers
{
    internal class AugmentedImageFileProvider : IAugmentedImageFileProvider
    {
        private ImageType _imageType;

        public string LayoutId { get; set; }

        internal AugmentedImageFileProvider(ImageType imageType)
        {
            if (imageType == null)
            {
                throw new ArgumentNullException(nameof(imageType));
            }

            _imageType = imageType;
        }

        public IEnumerable<string> GetFilesFrom(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            if (!Directory.Exists(folderPath))
            {
                throw new ToolException(ToolErrorCode.DirectoryNotFound, $"<{folderPath}> is not found");
            }

            var wantedImageCount = _imageType.SupportedModifiers.Count();
            var files = Directory.GetFiles(folderPath);

            if (files.Count() < wantedImageCount)
            {
                throw new ToolException(ToolErrorCode.InvalidDirectoryContent, $"Folder must contains at least {wantedImageCount} files");
            }

            return files;
        }

        public bool CheckEveryNeededFileArePresent(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentNullException(nameof(folderPath));
            }

            var wantedFiles = _imageType.SupportedModifiers.Select(x => _imageType.ComposeFileName(LayoutId, x));

            return wantedFiles.All(file =>
            {
                var currentFileName = Path.Combine(folderPath, file);

                return File.Exists(currentFileName);
            });
        }
    }
}
