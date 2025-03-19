using System;
using System.IO;
using Nemeio.Core.Extensions;
using Nemeio.Core.Services;

namespace Nemeio.LayoutGen.Models
{
    internal class LGImageProvider
    {
        const string EmbeddedPrefix = "emb://";
        const string ConfiguratorIconFolderPath = "/img/icons";
        const string ImagesFolder = "Images";

        public const string DefaultIconName = "default.svg";

        private readonly IDocument _documentService;

        public LGImageProvider(IDocument documentService)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        public Stream GetImageStream(string resource)
        {
            if (!resource.StartsWith(EmbeddedPrefix) && resource.IsFileUrl())
            {
                return LoadImageFileContent(resource);
            }
            else if (IsEmbeddedResource(resource))
            {
                return LoadImageFromEmbeddedResources(resource);
            }
            else if (IsConfiguratorResource(resource))
            {
                return LoadConfiguratorResource(resource);
            }

            //  Resource not found
            //  Return default icon

            return LoadImageFromEmbeddedResources(DefaultIconName);
        }

        private bool IsConfiguratorResource(string name)
        {
            return File.Exists(
                GetConfiguratorResourcePath(name)
            );
        }

        private bool IsEmbeddedResource(string name)
        {
            name = RemovePrefixIfNeeded(name);

            return Resources.Resources.ResourceExists($"{ImagesFolder}.{name}");
        }

        private string RemovePrefixIfNeeded(string name)
        {
            if (!name.StartsWith(EmbeddedPrefix))
            {
                return name;
            }

            var embLength = EmbeddedPrefix.Length;

            return name.Substring(embLength, name.Length - embLength);
        }

        private string GetConfiguratorResourcePath(string name)
        {
            name = RemovePrefixIfNeeded(name);

            return $"{_documentService.GetConfiguratorPath()}{ConfiguratorIconFolderPath}/{name}";
        }

        private Stream LoadImageFromEmbeddedResources(string resourceName)
        {
            var fileName = RemovePrefixIfNeeded(resourceName);

            return Resources.Resources.GetResourceStream($"{ImagesFolder}.{fileName}");
        }

        private Stream LoadConfiguratorResource(string name)
        {
            return LoadImageFileContent(
                GetConfiguratorResourcePath(name)
            );
        }

        private Stream LoadImageFileContent(string imagePath)
        {
            return File.OpenRead(imagePath);
        }
    }
}
