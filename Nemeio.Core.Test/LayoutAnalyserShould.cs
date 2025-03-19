using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Models.LayoutWarning;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    public class LayoutAnalyserShould
    {
        private const string WindowsNotpadPath  = "C:\\Windows\\notepad.exe";
        private const string WrongProgrammPath  = "C:/Program Files/MyProgram/myProgram.exe";

        [Test]
        public void LayoutAnalyser_01_01_Constructor_NullParameter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new LayoutAnalyser(null));
        }

        [Test]
        public void LayoutAnalyser_02_01_Analyse_WithValidLinkedApplicationPath_ReturnEmptyList()
        {
            var applicationLinks = new List<string>() {  };
            var layout = CreateLayout(new List<Key>(), applicationLinks);
            var analyser = new LayoutAnalyser(layout);

            var warnings = analyser.Analyse();

            warnings.Should().NotBeNull();
            warnings.Should().BeEmpty();
        }

        [Test]
        public void LayoutAnalyser_02_02_Analyse_WithInvalidLinkedApplicationPath_ReturnApplicationPathWarning()
        {
            var applicationLinks = new List<string>() { WrongProgrammPath };
            var layout = CreateLayout(new List<Key>(), applicationLinks);
            var analyser = new LayoutAnalyser(layout);

            var warnings = analyser.Analyse();

            warnings.Should().NotBeNull();
            warnings.Should().NotBeEmpty();
            warnings.Count().Should().Be(1);
            warnings.ElementAt(0).GetType().Should().Be(typeof(ApplicationPathWarning));
        }

        [Test]
        public void LayoutAnalyser_02_03_Analyse_WithValidKeyActionApplicationPath_ReturnEmptyList()
        {
            var keys = new List<Key>()
            {
                CreateKey(12, KeyboardModifier.Shift, WindowsNotpadPath)
            };

            var layout = CreateLayout(keys, new List<string>());
            var analyser = new LayoutAnalyser(layout);

            var warnings = analyser.Analyse();

            warnings.Should().NotBeNull();
            warnings.Should().BeEmpty();
        }

        [Test]
        public void LayoutAnalyser_02_04_Analyse_WithInvalidKeyActionApplicationPath_ReturnKeyApplicationPathWarning()
        {
            var keys = new List<Key>()
            {
                CreateKey(12, KeyboardModifier.Shift, WrongProgrammPath)
            };

            var layout = CreateLayout(keys, new List<string>());
            var analyser = new LayoutAnalyser(layout);

            var warnings = analyser.Analyse();

            warnings.Should().NotBeNull();
            warnings.Should().NotBeEmpty();
            warnings.Count().Should().Be(1);
            warnings.ElementAt(0).GetType().Should().Be(typeof(KeyApplicationPathWarning));
        }

        private ILayout CreateLayout(List<Key> keys, List<string> applicationLinks)
        {
            var screen = Mock.Of<IScreen>();

            return new Layout(
                new LayoutInfo(
                    new OsLayoutId(""),
                    false,
                    false,
                    applicationLinks,
                    false
                ),
                new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen),
                new byte[0],
                123,
                0,
                String.Empty,
                String.Empty,
                DateTime.Now,
                DateTime.Now,
                keys,
                LayoutId.NewLayoutId,
                null,
                false,
                true
            );
        }

        private Key CreateKey(int keyIndex, KeyboardModifier modifier, string subActionData)
        {
            return new Key()
            {
                Index = keyIndex,
                Font = FontProvider.GetDefaultFont(),
                Disposition = Enums.KeyDisposition.Single,
                Edited = false,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Display = "A",
                        Modifier = modifier,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(subActionData, KeyActionType.Application)
                        }
                    }
                }
            };
        }
    }
}
