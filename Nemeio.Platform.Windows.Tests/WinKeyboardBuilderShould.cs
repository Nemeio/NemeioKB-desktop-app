using System;
using System.Windows.Forms;
using FluentAssertions;
using Moq;
using Nemeio.Core.Enums;
using Nemeio.Core.Managers;
using Nemeio.Core.Services.Layouts;
using Nemeio.Platform.Windows.Keyboards;
using Nemeio.Platform.Windows.Layouts;
using Nemeio.Windows.Application.Resources;
using NUnit.Framework;

namespace Nemeio.Platform.Windows.Tests
{
    [TestFixture]
    public class WinKeyboardBuilderShould
    {
        private InputLanguage _azerty;

        private static readonly IntPtr AzertyHandle = (IntPtr)0x00000000040c040c;

        [SetUp]
        public void SetUp()
        {
            foreach (InputLanguage inpt in InputLanguage.InstalledInputLanguages)
            {
                if (inpt.Handle == AzertyHandle)
                {
                    _azerty = inpt;
                }
            }
        }

        [Test]
        [TestCase((uint)0x35)]
        [TestCase((uint)0x34)]
        [TestCase((uint)0x33)]
        [TestCase((uint)0x32)]
        public void GetValueForEmptyKey(uint scanCode)
        {
            if (PipelineChecker.RunningOnPipeline())
            {
                return;
            }

            var languageManager = Mock.Of<ILanguageManager>();
            var osLayoutIdBuilder = new WinOsLayoutIdBuilder(languageManager);
            var resourceLoader = new WinResourceLoader();
            var keyboardBuilder = new WinKeyboardBuilder(resourceLoader, osLayoutIdBuilder);
            var keyValue = keyboardBuilder.GetKeyValue(scanCode, KeyboardModifier.Shift | KeyboardModifier.AltGr, GetLayoutId(_azerty));

            keyValue.Should().Be(string.Empty);
        }

        private OsLayoutId GetLayoutId(InputLanguage inpt) => new WinOsLayoutId(inpt.Culture.Name, inpt.Handle);
    }
}
