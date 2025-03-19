using System.IO.Compression;
using System;
using System.IO;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Images.Jpeg;
using Nemeio.Core.Services.Layouts;
using Nemeio.Tools.Core.Files;
using Newtonsoft.Json;

namespace Nemeio.Tools.Image.ImagePackageBuilder.Packages.Writer
{
    internal class ImagePackageWriter : IImagePackageWriter
    {
        public const string DefaultOutputFileName = "jpg.package";

        private readonly IFileSystem _fileSystem;

        public ImagePackageWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public async Task WriteOnDiskAsync(JpegImagePackage package, string outputFilePath, LayoutFileDto layoutFile)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            LayoutFileDto targetLayoutFile = new LayoutFileDto()
            {
                Id = string.IsNullOrWhiteSpace(layoutFile?.Id) ? Guid.NewGuid().ToString() : layoutFile.Id,
                ImageBpp = 255,
                AssociatedId = layoutFile?.AssociatedId ?? String.Empty,
                Factory = layoutFile?.Factory ?? "0",
                DisableModifiers = layoutFile?.DisableModifiers ?? "1",
                IsHid = layoutFile?.IsHid ?? "1",
                //Set 2 by default for undefined
                BackgroundColor = layoutFile?.BackgroundColor ?? 2
            };


            var filePath = string.Empty;

            if (!string.IsNullOrEmpty(outputFilePath))
            {
                var folderPath = _fileSystem.GetDirectoryName(outputFilePath);
                if (!_fileSystem.DirectoryExists(folderPath))
                {
                    throw new InvalidOperationException($"Folder at path <{folderPath}> doesn't exists");
                }

                var fileName = _fileSystem.GetFileName(outputFilePath);
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new InvalidOperationException($"Path <{outputFilePath}> must contains file name");
                }

                filePath = outputFilePath;
            }
            else
            {
                filePath = Path.Combine(_fileSystem.GetCurrentDirectory(), DefaultOutputFileName);
            }

            var imgStream = new MemoryStream();
            var jsnStream = new MemoryStream();
            var finalResultStream = new MemoryStream();

            using (var ZipArchive = new ZipArchive(finalResultStream, ZipArchiveMode.Create, true))
            {
                var jsonEntry = ZipArchive.CreateEntry($"result/json.json");
                using (var entryStream = jsonEntry.Open())
                {
                    using (var sw = new StreamWriter(entryStream))
                    {
                        sw.Write(JsonConvert.SerializeObject(targetLayoutFile));
                    }
                }

                var imgEntry = ZipArchive.CreateEntry($"result/json.wallpaper.gz");
                using (var entryStream = imgEntry.Open())
                {
                    using (var writer = new BinaryWriter(entryStream))
                    {
                        package.Convert(writer);
                    }
                }
            }
            finalResultStream.Position = 0;
            //using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            //{
            //    fs.Write(finalResultStream.ToArray(), 0, finalResultStream.ToArray().Length);
            //}
            await _fileSystem.WriteAsync(filePath, finalResultStream.ToArray());

        }

    }
}
