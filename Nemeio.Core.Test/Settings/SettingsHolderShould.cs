using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Errors;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.Core.Settings;
using Nemeio.Core.Settings.Parser;
using Nemeio.Core.Settings.Providers;
using Nemeio.Core.Test.Settings;
using Nemeio.Core.Tools.Watchers;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class SettingsHolderShould
    {
        private ISettingsHolder _settingsHolder;
        private IDocument _mockDocumentService;
        private IErrorManager _mockErrorManager;
        private ISettingsProvider _settingsProvider;
        private IWatcherFactory _watcherFactory;
        private ILoggerFactory _loggerFactory;

        [SetUp]
        public void SetUp()
        {
            _mockDocumentService = Mock.Of<IDocument>();
            Mock.Get(_mockDocumentService)
                .Setup(x => x.UserNemeioFolder)
                .Returns(Path.GetTempPath());

            var fileSystem = new FileSystem.FileSystem();
            _settingsProvider = new SettingsProvider(new LoggerFactory(), new XmlSettingsParser(fileSystem));

            var mockFileWatcher = Mock.Of<IWatcher>();

            _watcherFactory = Mock.Of<IWatcherFactory>();
            Mock.Get(_watcherFactory)
                .Setup(x => x.CreateFileWatcher(It.IsAny<string>()))
                .Returns(mockFileWatcher);

            _loggerFactory = new LoggerFactory();
            _mockErrorManager = Mock.Of<IErrorManager>();
        }

        [Test]
        public void SettingsHolder_LoadSettings_WhenFileDoesntExists_ReturnNull()
        {
            _settingsHolder = new SettingsHolder(_loggerFactory, _mockDocumentService, _mockErrorManager, _settingsProvider, _watcherFactory, null);

            _settingsHolder.Settings.Should().BeNull();
        }

        [Test]
        public void SettingsHolder_LoadSettings_WhenFileCantBeParsed_ReturnNull()
        {
            using (var tmpFile = new TemporaryFile(GetTempSettingsFilePath()))
            {
                tmpFile.WriteAndClose("this_is_not_an_xml_content");

                _settingsHolder = new SettingsHolder(_loggerFactory, _mockDocumentService, _mockErrorManager, _settingsProvider, _watcherFactory, null);

                _settingsHolder.Settings.Should().BeNull();
            }
        }

        [Test]
        public void SettingsHolder_LoadSettings_WorksOk()
        {
            const int expectedApiPort               = 1234;
            const bool expectedSwaggerState         = true;

            //  Avoid dotnet to write 'True' instead of 'true' on XML file.
            var expectedSwaggerStateValue = $"{expectedSwaggerState}".ToLower();
            var settings = $"<settings><apiPort>{expectedApiPort}</apiPort><swaggerEnable>{expectedSwaggerStateValue}</swaggerEnable></settings>";

            using (var tmpFile = new TemporaryFile(GetTempSettingsFilePath()))
            {
                tmpFile.WriteAndClose(settings);

                _settingsHolder = new SettingsHolder(_loggerFactory, _mockDocumentService, _mockErrorManager, _settingsProvider, _watcherFactory, null);

                _settingsHolder.Settings.Should().NotBeNull();
                _settingsHolder.Settings.ApiPort.Should().NotBeNull();
                _settingsHolder.Settings.ApiPort.Value.Should().Be(expectedApiPort);
                _settingsHolder.Settings.SwaggerEnable.Should().NotBeNull();
                _settingsHolder.Settings.SwaggerEnable.Value.Should().Be(expectedSwaggerState);
            }
        }

        [Test]
        public async Task SettingsHolder_LoadSettings_WhenFileChange_Ok()
        {
            var watcher = new FakeFileWatcher();
            var watcherFactory = Mock.Of<IWatcherFactory>();
            Mock.Get(watcherFactory)
                .Setup((watcherFactory) => watcherFactory.CreateFileWatcher(It.IsAny<string>()))
                .Returns(watcher);

            using (var tmpFile = new TemporaryFile(GetTempSettingsFilePath()))
            {
                var apiPort = 1234;
                var swaggerEnable = true;

                var expectedSwaggerStateValue = $"{swaggerEnable}".ToLower();
                var settings = $"<settings><apiPort>{apiPort}</apiPort><swaggerEnable>{expectedSwaggerStateValue}</swaggerEnable></settings>";

                tmpFile.WriteAndClose(settings);

                _settingsHolder = new SettingsHolder(_loggerFactory, _mockDocumentService, _mockErrorManager, _settingsProvider, watcherFactory, null);

                //  We check first values
                _settingsHolder.Settings.Should().NotBeNull();
                _settingsHolder.Settings.ApiPort.Should().NotBeNull();
                _settingsHolder.Settings.ApiPort.Value.Should().Be(apiPort);
                _settingsHolder.Settings.SwaggerEnable.Should().NotBeNull();
                _settingsHolder.Settings.SwaggerEnable.Value.Should().Be(swaggerEnable);

                //  We change current settings
                apiPort = 9876;
                swaggerEnable = false;
                expectedSwaggerStateValue = $"{swaggerEnable}".ToLower();
                settings = $"<settings><apiPort>{apiPort}</apiPort><swaggerEnable>{expectedSwaggerStateValue}</swaggerEnable></settings>";

                tmpFile.WriteAndClose(settings);

                watcher.RaiseEvent();

                await Task.Delay(10);

                //  We check after changes
                _settingsHolder.Settings.Should().NotBeNull();
                _settingsHolder.Settings.ApiPort.Should().NotBeNull();
                _settingsHolder.Settings.ApiPort.Value.Should().Be(apiPort);
                _settingsHolder.Settings.SwaggerEnable.Should().NotBeNull();
                _settingsHolder.Settings.SwaggerEnable.Value.Should().Be(swaggerEnable);
            }
        }

        private string GetTempSettingsFilePath()
        {
            var tempDirectoryPath = _mockDocumentService.UserNemeioFolder;
            var tempFilePath = Path.Combine(tempDirectoryPath, SettingsHolder.SettingsFilename);

            return tempFilePath;
        }

        private class FakeFileWatcher : IWatcher
        {
            public event EventHandler OnChanged;

            public void RaiseEvent() => OnChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
