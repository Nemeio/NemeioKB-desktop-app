using System;
using System.IO;
using System.Xml.Serialization;
using Nemeio.Core.DataModels;
using Nemeio.Core.FileSystem;

namespace Nemeio.Core.Settings.Parser
{
    public sealed class XmlSettingsParser : ISettingsParser
    {
        private readonly IFileSystem _fileSystem;

        public XmlSettingsParser(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public DevelopmentSettings Parse(string filePath)
        {
            if (!_fileSystem.FileExists(filePath))
            {
                throw new ArgumentException($"<{filePath}> doesn't exists");
            }

            using (var stream = new StreamReader(filePath))
            {
                var xmlSerializer = new XmlSerializer(typeof(DevelopmentSettings));
                var loadedSettings = (DevelopmentSettings)xmlSerializer.Deserialize(stream);

                stream.Close();

                return loadedSettings;
            }
        }
    }
}
