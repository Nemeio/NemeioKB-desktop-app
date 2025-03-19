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
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    [TestFixture]
    public class LayoutShould
    {
        private ILayout _layout;

        [SetUp]
        public void SetUp()
        {
            var layoutInfo = new LayoutInfo(
                new OsLayoutId(""),
                false,
                false);

            var screen = Mock.Of<IScreen>();

            _layout = new Layout(
                layoutInfo,
                new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen),
                new byte[0],
                123,
                0,
                string.Empty,
                String.Empty,
                DateTime.Now, 
                DateTime.Now, 
                new List<Key>(),
                LayoutId.NewLayoutId,
                null,
                true
            );
        }

        /*[Test]
        public void Layout_Constructor_01_01_NullImage_Throws()
        {
            int position = 1;
            var osLayoutId = new OsLayoutId("TestLayoutId");
            byte[] nullImage = null;
            var layoutInfo = new LayoutInfo(
                new OsLayoutId("TestLayoutId"),
                false,
                true);
            Assert.Throws<NullReferenceException>(() => new Layout(layoutInfo, nullImage, NemeioConstants.DefaultCategoryId,
                position, osLayoutId.Name, DateTime.Now, DateTime.Now, new List<Key>(),
                FontProvider.GetDefaultFont()));
        }*/

        [Test]
        [TestCase(KeyboardLiterals.Alt)]
        [TestCase(KeyboardLiterals.AltGr)]
        public void ComputeSASAllOnOneKeys_01_03_WorksOk(string alt)
        {
            var keys = new List<Key>();
            keys.Add(new Key()
            {
                Index = 32,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Ctrl),
                            KeySubAction.CreateModifierAction(alt),
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Delete)
                        }
                    }
                }
            });

            _layout.Keys = keys;

            _layout.SpecialSequences.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Count().Should().Be(1);
            _layout.SpecialSequences.WinSAS.ElementAt(0).ElementAt(0).Should().Be(32);
        }

        [Test]
        [TestCase(KeyboardLiterals.Alt)]
        [TestCase(KeyboardLiterals.AltGr)]
        public void ComputeSASAllOnOneKeysButNotValid_01_04_WorksOk(string alt)
        {
            var keys = new List<Key>();
            keys.Add(new Key()
            {
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Ctrl),
                            new KeySubAction("a", KeyActionType.Unicode),
                            KeySubAction.CreateModifierAction(alt),
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Delete)
                        }
                    }
                }
            });

            _layout.Keys = keys;

            _layout.SpecialSequences.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Count().Should().Be(0);
        }

        [Test]
        [TestCase(KeyboardLiterals.Alt)]
        [TestCase(KeyboardLiterals.AltGr)]
        public void ComputeSASAllOnOneKeysButNotOnGoodOrder_01_05_WorksOk(string alt)
        {
            var keys = new List<Key>();
            keys.Add(new Key()
            {
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Delete),
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Ctrl),
                            KeySubAction.CreateModifierAction(alt),
                        }
                    }
                }
            });

            _layout.Keys = keys;

            _layout.SpecialSequences.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Count().Should().Be(0);
        }

        [Test]
        [TestCase(KeyboardLiterals.Alt)]
        [TestCase(KeyboardLiterals.AltGr)]
        public void ComputeSASAllOnOneKeysButNotOnModifierNone_01_06_WorksOk(string alt)
        {
            var keys = new List<Key>();
            keys.Add(new Key()
            {
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Ctrl),
                            KeySubAction.CreateModifierAction(alt),
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Delete)
                        }
                    }
                }
            });

            _layout.Keys = keys;

            _layout.SpecialSequences.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Count().Should().Be(0);
        }

        [Test]
        [TestCase(KeyboardLiterals.Alt)]
        [TestCase(KeyboardLiterals.AltGr)]
        public void CompuuteSASEachOnDifferentKeys_01_07_WorksOk(string alt)
        {
            var keys = new List<Key>();
            keys.Add(new Key()
            {
                Index = 14,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Ctrl),
                        }
                    }
                }
            });
            keys.Add(new Key()
            {
                Index = 63,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(alt),
                        }
                    }
                }
            });
            keys.Add(new Key()
            {
                Index = 21,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Delete),
                        }
                    }
                }
            });

            _layout.Keys = keys;

            _layout.SpecialSequences.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Count().Should().Be(2);
        }

        [Test]
        [TestCase(KeyboardLiterals.Alt)]
        [TestCase(KeyboardLiterals.AltGr)]
        public void CompuuteSASEachOnDifferentKeysButNotOnNoneModifier_01_08_WorksOk(string alt)
        {
            var keys = new List<Key>();
            keys.Add(new Key()
            {
                Index = 14,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Ctrl),
                        }
                    }
                }
            });
            keys.Add(new Key()
            {
                Index = 63,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(alt),
                        }
                    }
                }
            });
            keys.Add(new Key()
            {
                Index = 21,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            KeySubAction.CreateModifierAction(KeyboardLiterals.Delete),
                        }
                    }
                }
            });

            _layout.Keys = keys;

            _layout.SpecialSequences.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Should().NotBeNull();
            _layout.SpecialSequences.WinSAS.Count().Should().Be(0);
        }
    }
}
