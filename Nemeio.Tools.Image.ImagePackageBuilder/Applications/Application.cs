using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Images.Jpeg;
using Nemeio.Core.Images.Jpeg.Builder;
using Nemeio.Core.PackageUpdater.Firmware;
using Nemeio.Tools.Core.Files;
using Nemeio.Tools.Image.ImagePackageBuilder.Exceptions;
using Nemeio.Tools.Image.ImagePackageBuilder.Files;
using Nemeio.Tools.Image.ImagePackageBuilder.Packages.Writer;
using Newtonsoft.Json;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Applications
{
    internal sealed class Application : IImagePackageBuilderApplication
    {
        private readonly IFileSystem _fileSystem;
        private readonly IImagePackageWriter _packageWriter;
        private readonly IJpegImagePackageBuilder _imagePackageBuilder;

        public ApplicationStartupSettings Settings { get; set; }

        public Application(IFileSystem fileSystem, IImagePackageWriter packageWriter, IJpegImagePackageBuilder imagePackageBuilder)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _packageWriter = packageWriter ?? throw new ArgumentNullException(nameof(packageWriter));
            _imagePackageBuilder = imagePackageBuilder ?? throw new ArgumentNullException(nameof(imagePackageBuilder));
        }

        public async Task RunAsync()
        {
            var compressionType = Enum.Parse<PackageCompressionType>(Settings.CompressionType, ignoreCase: true);

            if (Settings.ImagesPath == null || !Settings.ImagesPath.Any())
            {
                throw new NotEnoughInputFileException($"You must have at least 1 image");
            }

            var inputFiles = Settings
                .ImagesPath
                .Select(path => new BmpInputFile(_fileSystem, path))
                .ToList();

            var imageFiles = new List<JpegImageData>();

            foreach (var file in inputFiles)
            {
                using (var img = System.Drawing.Image.FromFile(file.FilePath))
                {
                    img.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipX);
                    var imageFile = new JpegImageData(imageToByteArray(img), compressionType);
                    imageFiles.Add(imageFile);
                }
            }
            var jsonContent = new LayoutFileDto();
            try
            {
                if (Settings.JSon != null)
                {
                    jsonContent = JsonConvert.DeserializeObject<LayoutFileDto>(Settings.JSon);
                }
            }
            catch (Exception)
            {
                throw new InvalidJSonStringException($"JSon is invlid <{Settings.JSon}>");
            }

            var package = _imagePackageBuilder.CreatePackage(imageFiles);

            try
            {
                await _packageWriter.WriteOnDiskAsync(package, Settings.OutputPath, jsonContent);
            }
            catch (Exception ex)
            {
                throw new WritePackageFailedException($"Failed to write package at path <{Settings.OutputPath}>");
            }
        }

        private static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}
