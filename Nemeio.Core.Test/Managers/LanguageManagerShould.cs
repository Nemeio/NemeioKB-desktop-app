using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Core.Applications;
using Nemeio.Core.Managers;
using Nemeio.Core.Test.Fakes;
using NUnit.Framework;

namespace Nemeio.Core.Test.Managers
{
    [TestFixture]
    public class LanguageManagerShould
    {
        private bool LanguagePropertyCalled = false;

        private FakeLanguageManager _fakeLanguageManager;
        private IApplicationSettingsProvider _applicationSettingsManager;

        [SetUp]
        public void SetUp()
        {
            LanguagePropertyCalled = false;

            _applicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();

            Mock.Get(_applicationSettingsManager)
                .Setup(x => x.Language)
                .Callback(() => LanguagePropertyCalled = true)
                .Returns(new CultureInfo("en-US"));

            _fakeLanguageManager = new FakeLanguageManager(new LoggerFactory());
            _fakeLanguageManager.InjectApplicationSettingsManager(_applicationSettingsManager);
            _fakeLanguageManager.Start();
        }

        [Test]
        public void LanguageManager_01_01_ResourcesAssemblyPath_ReturnPathToNemeioCoreResource_WorksOk()
        {
            _fakeLanguageManager.GetResourcesAssemblyPath().Should().Be("Nemeio.Core.Resources.Languages");
        }

        [Test]
        [TestCase("fr-FR.xml")]
        [TestCase("en-US.xml")]
        public void LanguageManager_02_01_ResourceExists_WhenFileIsPresent_ReturnTrue(string filename)
        {
            _fakeLanguageManager.ResourceExists(filename).Should().BeTrue();
        }

        [Test]
        [TestCase("it-IT.xml")]
        [TestCase("es-ES.xml")]
        public void LanguageManager_02_02_ResourceExists_WhenFileIsNotPresent_ReturnFalse(string filename)
        {
            _fakeLanguageManager.ResourceExists(filename).Should().BeFalse();
        }

        [Test]
        public void LanguageManager_03_01_SetCurrentCultureInfo_CallApplicationSettingsUpdate_WorksOk()
        {
            _fakeLanguageManager.SetCurrentCultureInfo(new CultureInfo("fr-FR"));

            LanguagePropertyCalled.Should().BeTrue();
        }

        [Test]
        public void LanguageManager_03_02_SetCurrentCultureInfo_LanguageMustBeChanged_WorksOk()
        {
            var beforeUpdate = _fakeLanguageManager.CurrentLanguage;

            _fakeLanguageManager.SetCurrentCultureInfo(new CultureInfo("fr-FR"));

            _fakeLanguageManager.CurrentLanguage.Should().NotBeSameAs(beforeUpdate);
        }

        [Test]
        public void LanguageManager_03_03_SetCurrentCultureInfo_LanguageDoesntMustChange_WhenLanguageIsUnknown()
        {
            var beforeUpdate = _fakeLanguageManager.CurrentLanguage;

            _fakeLanguageManager.SetCurrentCultureInfo(_applicationSettingsManager.Language);

            _fakeLanguageManager.CurrentLanguage.Should().BeEquivalentTo(beforeUpdate);
        }

        [Test]
        public void LanguageManager_04_01_GetLocalizedValue_WorksOk()
        {
            var fakeCultureInfo = new CultureInfo(_fakeLanguageManager.FakeCulture);

            _fakeLanguageManager.SetCurrentCultureInfo(fakeCultureInfo);

            _fakeLanguageManager.GetLocalizedValue(Core.DataModels.Locale.StringId.ApplicationTitleName).Should().Be("NemeioFakeTranslate");
        }

        [Test]
        public void LanguageManager_05_01_LoadLocalizableFile_WhenTranslateNotSupported_MustFallbackOnEnglish()
        {
            var notSupportedCultureCode = "zt-ZK";
            var notSupportedCultureInfo = new CultureInfo(notSupportedCultureCode);

            _fakeLanguageManager.SetCurrentCultureInfo(notSupportedCultureInfo);

            _fakeLanguageManager.CurrentCulture.Name.Should().Be(LanguageManager.DefaultLanguageCode);
        }

        [Test]
        public void LanguageManager_06_01_SetCultureInfo_WithoutCallInject_ThrowsArgumentNullException()
        {
            var newCultureInfo      = new CultureInfo("fr-FR");
            var languageManager     = new LanguageManager(new LoggerFactory());

            Assert.Throws<ArgumentNullException>(() => 
            {
                languageManager.SetCurrentCultureInfo(newCultureInfo);
            });

            languageManager.InjectApplicationSettingsManager(_applicationSettingsManager);

            Assert.DoesNotThrow(() => languageManager.SetCurrentCultureInfo(newCultureInfo));
        }

        [Test]
        public void LanguageManager_06_02_GetCurrentCultureInfo_WithoutCallInject_ThrowsArgumentNullException()
        {
            var languageManager = new LanguageManager(new LoggerFactory());

            Assert.Throws<ArgumentNullException>(() => languageManager.GetCurrentCultureInfo());

            languageManager.InjectApplicationSettingsManager(_applicationSettingsManager);

            Assert.DoesNotThrow(() => languageManager.GetCurrentCultureInfo());
        }

        [Test]
        public void LanguageManager_06_03_Start_WithoutCallInject_ThrowsArgumentNullException()
        {
            var languageManager = new LanguageManager(new LoggerFactory());

            Assert.Throws<ArgumentNullException>(() => languageManager.Start());

            languageManager.InjectApplicationSettingsManager(_applicationSettingsManager);

            Assert.DoesNotThrow(() => languageManager.Start());
        }

        [Test]
        public void LanguageManager_07_01_SelectMainLanguage_WhenUserAlreadyDidIt_ThrowsInvalidOperationException()
        {
            var selectedCultureInfo = new CultureInfo("fr-FR");
            var languageManager = new LanguageManager(new LoggerFactory());
            languageManager.InjectApplicationSettingsManager(_applicationSettingsManager);
            languageManager.Start();

            Assert.Throws<InvalidOperationException>(() => languageManager.SelectMainLanguage(selectedCultureInfo));
        }

        [Test]
        public void LanguageManager_07_02_SelectMainLanguage_NeedUserSelection_UpdateCurrentCulture()
        {
            var selectedCultureInfo = new CultureInfo("fr-FR");
            var languageManager = new LanguageManager(new LoggerFactory());
            var testableEvent = new TestableLanguageManagerEvent(languageManager);
            var mockApplicationSettingsManager = Mock.Of<IApplicationSettingsProvider>();

            Mock.Get(mockApplicationSettingsManager)
                .Setup(x => x.Language)
                .Returns(() => null);

            testableEvent.Register();

            languageManager.InjectApplicationSettingsManager(mockApplicationSettingsManager);
            languageManager.Start();

            testableEvent.LanguageDueCareCalled.Should().BeTrue();
            testableEvent.ResetValues();

            Assert.DoesNotThrow(() => languageManager.SelectMainLanguage(selectedCultureInfo));
      
            testableEvent.LanguageChangedCareCalled.Should().BeTrue();
            testableEvent.Unregister();
        }

        class TestableLanguageManagerEvent
        {
            private LanguageManager _languageManager;

            public bool LanguageDueCareCalled { get; private set; }

            public bool LanguageChangedCareCalled { get; private set; }

            public TestableLanguageManagerEvent(LanguageManager languageManager)
            {
                _languageManager = languageManager;
            }

            public void Register()
            {
                _languageManager.LanguageDueCare += LanguageManager_LanguageDueCare;
                _languageManager.LanguageChanged += LanguageManager_LanguageChanged;
            }

            public void Unregister()
            {
                _languageManager.LanguageDueCare -= LanguageManager_LanguageDueCare;
                _languageManager.LanguageChanged -= LanguageManager_LanguageChanged;
            }

            public void ResetValues()
            {
                LanguageDueCareCalled = false;
                LanguageChangedCareCalled = false;
            }

            private void LanguageManager_LanguageChanged(object sender, EventArgs e) => LanguageChangedCareCalled = true;

            private void LanguageManager_LanguageDueCare(object sender, EventArgs e) => LanguageDueCareCalled = true;
        }
    }
}
