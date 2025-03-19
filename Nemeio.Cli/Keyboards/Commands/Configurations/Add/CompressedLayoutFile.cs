using System;
using System.Linq;
using System.Threading.Tasks;
using Nemeio.Core.FileSystem;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Services.Layouts;
using Nemeio.Tools.Core.Files;
using Newtonsoft.Json;

namespace Nemeio.Cli.Keyboards.Commands.Configurations.Add
{
    internal sealed class CompressedLayoutFile : File
    {
        private const string LayoutFileExtension = ".zip";
        private const int WaitedFileCount = 2;
        private const string JsonExtension = ".json";
        private const string ImageExtension = ".gz";

        private readonly IFileSystem _fileSystem;

        public CompressedLayoutFile(IFileSystem fileSystem, IFile file) 
            : base(file) 
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

            var extension = System.IO.Path.GetExtension(Path);
            if (extension != LayoutFileExtension)
            {
                throw new System.IO.InvalidDataException($"Layout file must be a {LayoutFileExtension}");
            }
        }

        public async Task<ILayout> LoadLayoutAsync()
        {
            var directory = Extract();

            var content = directory
                .GetEntries()
                .OfType<IFile>()
                .ToList();

            if (content.Count != WaitedFileCount)
            {
                throw new InvalidOperationException($"Number of files found is <{content.Count}> but waited <{WaitedFileCount}>");
            }

            var jsonFile = content.FirstOrDefault(x => x.Extension.Equals(JsonExtension));
            if (jsonFile == null)
            {
                throw new System.IO.FileNotFoundException($"No JSON file found");
            }

            var imageFile = content.FirstOrDefault(x => x.Extension.Equals(ImageExtension));
            if (imageFile == null)
            {
                throw new System.IO.FileNotFoundException($"No image file found");
            }

            var jsonContent = await jsonFile.ReadContentAsync();
            var dto = JsonConvert.DeserializeObject<LayoutFileDto>(jsonContent);

            var image = await imageFile.ReadByteArrayContentAsync();

            var keyboardLayout = new KeyboardLayout(dto, image);

            return keyboardLayout;
        }

        private IDirectory Extract()
        {
            var directoryName = Guid.NewGuid().ToString();
            var directoryPath = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(), 
                directoryName
            );
            
            var directory = _fileSystem.Unzip(Path, directoryPath);

            return directory;
        }
    }
}
