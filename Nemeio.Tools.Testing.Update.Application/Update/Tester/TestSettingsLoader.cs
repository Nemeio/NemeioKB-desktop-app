using System;
using Nemeio.Core.FileSystem;
using Nemeio.Tools.Testing.Update.Core.Update.Tester.Exceptions;
using Nemeio.Tools.Testing.Update.Core.Update.Tester.Settings;

namespace Nemeio.Tools.Testing.Update.Application.Update.Tester
{
    public class TestSettingsLoader : ITestSettingsLoader
    {
        private readonly ITestSettingsParser _settingsParser;
        private readonly IFileSystem _fileSystem;

        public TestSettingsLoader(IFileSystem fileSystem, ITestSettingsParser settingsParser)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _settingsParser = settingsParser ?? throw new ArgumentNullException(nameof(settingsParser));
        }

        public TestSettings LoadSettings(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!_fileSystem.FileExists(path))
            {
                throw new MissingSettingsFileException();
            }

            var settingsFileContent = _fileSystem.ReadTextAsync(path).GetAwaiter().GetResult();

            return _settingsParser.Parse(settingsFileContent);
        }
    }
}
