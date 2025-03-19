using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Nemeio.Core.Services.AppSettings;
using Nemeio.Infrastructure.Repositories;
using NUnit.Framework;

namespace Nemeio.Infrastructure.Test
{
    [TestFixture]
    public class ApplicationSettingsDbRepositoryShould : DbRepositoryTestBase
    {
        private ApplicationSettingsDbRepository _applicationSettingsDbRepository;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _applicationSettingsDbRepository = new ApplicationSettingsDbRepository(new LoggerFactory(), _databaseAccessFactory);
        }

        [Test]
        public void ApplicationSettingsDbRepository_ReadApplicationSettings_Ok()
        {
            var applicationSettings = _applicationSettingsDbRepository.ReadApplicationSettings();

            applicationSettings.Should().NotBeNull();
            applicationSettings.Language.Should().BeNull();
            applicationSettings.AugmentedImageEnable.Should().BeTrue();
            applicationSettings.ShowGrantPrivilegeWindow.Should().BeTrue();
        }

        [Test]
        public void ApplicationSettingsDbRepository_SaveApplicationSettings_Ok()
        {
            var applicationSettings = new ApplicationSettings(CultureInfo.CurrentUICulture, true, true, null, string.Empty);

            _applicationSettingsDbRepository.SaveApplicationSettings(applicationSettings);

            applicationSettings = _applicationSettingsDbRepository.ReadApplicationSettings();
            applicationSettings.Should().NotBeNull();
            applicationSettings.Language.Should().Be(CultureInfo.CurrentUICulture);
        }

        [Test]
        public void ApplicationSettingsDbRepository_SaveApplicationSettings_UpdateOk()
        {
            var cultureFr = CultureInfo.GetCultureInfo("en-US");
            var cultureUs = CultureInfo.GetCultureInfo("fr-FR");

            var applicationSettings = new ApplicationSettings(cultureFr, true, true, null, string.Empty);
            _applicationSettingsDbRepository.SaveApplicationSettings(applicationSettings);

            applicationSettings = _applicationSettingsDbRepository.ReadApplicationSettings();
            applicationSettings.Language = cultureUs;
            applicationSettings.AugmentedImageEnable = false;
            applicationSettings.ShowGrantPrivilegeWindow = false;

            _applicationSettingsDbRepository.SaveApplicationSettings(applicationSettings);

            applicationSettings = _applicationSettingsDbRepository.ReadApplicationSettings();

            applicationSettings.Should().NotBeNull();
            applicationSettings.Language.Should().Be(cultureUs);
            applicationSettings.AugmentedImageEnable.Should().Be(false);
            applicationSettings.ShowGrantPrivilegeWindow.Should().Be(false);
        }

        [Test]
        public void ApplicationSettingsDbRepository_SaveApplicationSettings_NullError()
        {
            TestDelegate action = () => _applicationSettingsDbRepository.SaveApplicationSettings(null);

            Assert.Throws<InvalidOperationException>(action);
        }


        [Test]
        public void ApplicationSettingsDbRepository_SaveApplicationSettings_NullLanguage_NotThrowException()
        {
            var applicationSettings = new ApplicationSettings(null, true, true, null, string.Empty);

            TestDelegate action = () => _applicationSettingsDbRepository.SaveApplicationSettings(applicationSettings);

            Assert.DoesNotThrow(action);
        }
    }
}
