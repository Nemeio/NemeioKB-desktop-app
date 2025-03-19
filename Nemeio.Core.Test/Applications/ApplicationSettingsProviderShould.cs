using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Nemeio.Core.Applications;
using Nemeio.Core.Services.AppSettings;
using NUnit.Framework;

namespace Nemeio.Core.Test.Applications
{
    [TestFixture]
    public sealed class ApplicationSettingsProviderShould
    {
        private IApplicationSettingsDbRepository _mockApplicationSettingsDbRepository;
        private ApplicationSettingsProvider _applicationSettingsProvider;

        [SetUp]
        public void SetUp()
        {
            _mockApplicationSettingsDbRepository = Mock.Of<IApplicationSettingsDbRepository>();

            _applicationSettingsProvider = new ApplicationSettingsProvider(_mockApplicationSettingsDbRepository);
        }

        [Test]
        public void ApplicationSettingsProvider_Constructor_Ok()
        {
            Assert.Throws<ArgumentNullException>(() => new ApplicationSettingsProvider(null));
            Assert.DoesNotThrow(() => new ApplicationSettingsProvider(_mockApplicationSettingsDbRepository));
        }

        [Test]
        public void ApplicationSettingsProvider_ApplicationSettings_Ok()
        {
            var readApplicationSettingsCalled = false;
            var applicationSettings = new ApplicationSettings(new CultureInfo("en-US"), false, false, null, string.Empty);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.ReadApplicationSettings())
                .Callback(() => readApplicationSettingsCalled = true)
                .Returns(applicationSettings);

            var settings = _applicationSettingsProvider.ApplicationSettings;

            settings.Should().NotBeNull();
            settings.Should().Be(applicationSettings);
            readApplicationSettingsCalled.Should().BeTrue();
        }

        [Test]
        public void ApplicationSettingsProvider_Get_Language_Ok()
        {
            var readApplicationSettingsCalled = false;
            var languageSettings = new CultureInfo("en-US");
            var applicationSettings = new ApplicationSettings(languageSettings, false, false, null, string.Empty);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.ReadApplicationSettings())
                .Callback(() => readApplicationSettingsCalled = true)
                .Returns(applicationSettings);

            var language = _applicationSettingsProvider.Language;

            language.Should().NotBeNull();
            language.Should().Be(languageSettings);
            readApplicationSettingsCalled.Should().BeTrue();
        }

        [Test]
        public async Task ApplicationSettingsProvider_Set_Language_Ok()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var writeApplicationSettingsCalled = false;
            var eventRaised = false;

            var applicationSettings = new ApplicationSettings(new CultureInfo("en-US"), false, false, null, string.Empty);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.ReadApplicationSettings())
                .Returns(applicationSettings);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.SaveApplicationSettings(It.IsAny<ApplicationSettings>()))
                .Callback(() => writeApplicationSettingsCalled = true);

            _applicationSettingsProvider.LanguageChanged += delegate
            {
                eventRaised = true;

                try
                {
                    semaphore.Release();
                }
                catch (Exception) { }
            };

            _applicationSettingsProvider.Language = new CultureInfo("fr-FR");

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }

            writeApplicationSettingsCalled.Should().BeTrue();
            eventRaised.Should().BeTrue();
        }

        [Test]
        public void ApplicationSettingsProvider_Get_AugmentedImageEnable_Ok()
        {
            var readApplicationSettingsCalled = false;
            var augmentedImageEnableSettings = true;
            var applicationSettings = new ApplicationSettings(new CultureInfo("en-US"), augmentedImageEnableSettings, false, null, string.Empty);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.ReadApplicationSettings())
                .Callback(() => readApplicationSettingsCalled = true)
                .Returns(applicationSettings);

            var augmentedImageEnable = _applicationSettingsProvider.AugmentedImageEnable;

            augmentedImageEnable.Should().Be(augmentedImageEnableSettings);
            readApplicationSettingsCalled.Should().BeTrue();
        }

        [Test]
        public async Task ApplicationSettingsProvider_Set_AugmentedImageEnable_Ok()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var writeApplicationSettingsCalled = false;
            var eventRaised = false;

            var applicationSettings = new ApplicationSettings(new CultureInfo("en-US"), false, false, null, string.Empty);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.ReadApplicationSettings())
                .Returns(applicationSettings);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.SaveApplicationSettings(It.IsAny<ApplicationSettings>()))
                .Callback(() => writeApplicationSettingsCalled = true);

            _applicationSettingsProvider.AugmentedImageEnableChanged += delegate
            {
                eventRaised = true;

                try
                {
                    semaphore.Release();
                }
                catch (Exception) { }
            };

            _applicationSettingsProvider.AugmentedImageEnable = true;

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }

            writeApplicationSettingsCalled.Should().BeTrue();
            eventRaised.Should().BeTrue();
        }

        [Test]
        public void ApplicationSettingsProvider_Get_ShowGrantPrivilegeWindow_Ok()
        {
            var readApplicationSettingsCalled = false;
            var showGrantPrivilegeSettings = true;
            var applicationSettings = new ApplicationSettings(new CultureInfo("en-US"), false, showGrantPrivilegeSettings, null, string.Empty);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.ReadApplicationSettings())
                .Callback(() => readApplicationSettingsCalled = true)
                .Returns(applicationSettings);

            var showGrantPrivilege = _applicationSettingsProvider.ShowGrantPrivilegeWindow;

            showGrantPrivilege.Should().Be(showGrantPrivilegeSettings);
            readApplicationSettingsCalled.Should().BeTrue();
        }

        [Test]
        public async Task ApplicationSettingsProvider_Set_ShowGrantPrivilegeWindow_Ok()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var writeApplicationSettingsCalled = false;
            var eventRaised = false;

            var applicationSettings = new ApplicationSettings(new CultureInfo("en-US"), false, false, null, string.Empty);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.ReadApplicationSettings())
                .Returns(applicationSettings);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.SaveApplicationSettings(It.IsAny<ApplicationSettings>()))
                .Callback(() => writeApplicationSettingsCalled = true);

            _applicationSettingsProvider.ShowGrantPrivilegeWindowChanged += delegate
            {
                eventRaised = true;

                try
                {
                    semaphore.Release();
                }
                catch (Exception) { }
            };

            _applicationSettingsProvider.ShowGrantPrivilegeWindow = true;

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }

            writeApplicationSettingsCalled.Should().BeTrue();
            eventRaised.Should().BeTrue();
        }

        [Test]
        public void ApplicationSettingsProvider_Get_UpdateTo_Ok()
        {
            var readApplicationSettingsCalled = false;
            var updateToSettings = new Version("1.0.0");
            var applicationSettings = new ApplicationSettings(new CultureInfo("en-US"), false, false, updateToSettings, string.Empty);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.ReadApplicationSettings())
                .Callback(() => readApplicationSettingsCalled = true)
                .Returns(applicationSettings);

            var updateTo = _applicationSettingsProvider.UpdateTo;

            updateTo.Should().NotBeNull();
            updateTo.Should().Be(updateToSettings);
            readApplicationSettingsCalled.Should().BeTrue();
        }

        [Test]
        public async Task ApplicationSettingsProvider_Set_UpdateTo_Ok()
        {
            var semaphore = new SemaphoreSlim(0, 1);
            var writeApplicationSettingsCalled = false;
            var eventRaised = false;

            var applicationSettings = new ApplicationSettings(new CultureInfo("en-US"), false, false, null, string.Empty);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.ReadApplicationSettings())
                .Returns(applicationSettings);

            Mock.Get(_mockApplicationSettingsDbRepository)
                .Setup(x => x.SaveApplicationSettings(It.IsAny<ApplicationSettings>()))
                .Callback(() => writeApplicationSettingsCalled = true);

            _applicationSettingsProvider.UpdateToChanged += delegate
            {
                eventRaised = true;

                try
                {
                    semaphore.Release();
                }
                catch (Exception) { }
            };

            _applicationSettingsProvider.UpdateTo = null;

            var success = await semaphore.WaitAsync(TimeSpan.FromSeconds(2));
            if (!success)
            {
                throw new InvalidOperationException("Never relase semaphore");
            }

            writeApplicationSettingsCalled.Should().BeTrue();
            eventRaised.Should().BeTrue();
        }
    }
}
