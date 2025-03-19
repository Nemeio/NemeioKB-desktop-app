using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Keys;
using Nemeio.Core.Layouts.Color;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Managers;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;
using NUnit.Framework;

namespace Nemeio.Core.Test
{
    public class KeystrokeInterpreterShould
    {
        private ILayout _customLayout;

        private Key _shiftKey;
        private Key _altGrKey;
        private Key _zeroKey;
        private Key _shiftAndAltGr;
        private Key _aKey;
        private Key _functionKey;
        private Key _keyWithFiveActions;

        private int _shiftKeyIndex              = 1;
        private int _altGrKeyIndex              = 2;
        private int _shiftAndAltGrIndex         = 3;
        private int _zeroKeyIndex               = 4;
        private int _aKeyIndex                  = 5;
        private int _functionKeyIndex           = 6;
        private int _fiveActionsKeyIndex        = 7;

        private KeystrokeInterpreter _keystrokeInterpreter;

        [SetUp]
        public void SetUp()
        {
            _zeroKey = new Key()
            {
                Index = _zeroKeyIndex,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("à", KeyActionType.Unicode)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("0", KeyActionType.Unicode)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.AltGr,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("@", KeyActionType.Unicode)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift | KeyboardModifier.AltGr,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("LDLC", KeyActionType.Unicode)
                        }
                    }
                }
            };

            _aKey = new Key()
            {
                Index = _aKeyIndex,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("a", KeyActionType.Unicode)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("A", KeyActionType.Unicode)
                        }
                    }
                }
            };

            _shiftKey = new Key()
            {
                Index = _shiftKeyIndex,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.Shift, KeyActionType.Special)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.Shift, KeyActionType.Special)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.AltGr,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.Shift, KeyActionType.Special)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift | KeyboardModifier.AltGr,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.Shift, KeyActionType.Special)
                        }
                    }
                }
            };

            _altGrKey = new Key()
            {
                Index = _altGrKeyIndex,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.AltGr, KeyActionType.Special)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.AltGr, KeyActionType.Special)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.AltGr,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.AltGr, KeyActionType.Special)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift | KeyboardModifier.AltGr,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.AltGr, KeyActionType.Special)
                        }
                    }
                }
            };

            _shiftAndAltGr = new Key()
            {
                Index = _shiftAndAltGrIndex,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.Shift, KeyActionType.Special),
                            new KeySubAction(KeyboardLiterals.AltGr, KeyActionType.Special)
                        }
                    }
                }
            };

            _functionKey = new Key()
            {
                Index = _functionKeyIndex,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction(KeyboardLiterals.Fn, KeyActionType.Special)
                        }
                    }
                }
            };

            _keyWithFiveActions = new Key()
            {
                Index = _fiveActionsKeyIndex,
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("1", KeyActionType.Unicode)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("2", KeyActionType.Unicode)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.AltGr,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("3", KeyActionType.Unicode)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Shift | KeyboardModifier.AltGr,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("4", KeyActionType.Unicode)
                        }
                    },
                    new KeyAction()
                    {
                        Modifier = KeyboardModifier.Function,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("5", KeyActionType.Unicode)
                        }
                    }
                }
            };

            var customKeys = new List<Key>()
            {
                _zeroKey,
                _shiftKey,
                _altGrKey,
                _shiftAndAltGr,
                _aKey,
                _functionKey,
                _keyWithFiveActions
            };

            var screen = Mock.Of<IScreen>();
            var imageInfo = new LayoutImageInfo(HexColor.Black, FontProvider.GetDefaultFont(), screen);

            _customLayout = new Layout(
                new LayoutInfo(new OsLayoutId(""), false, false),
                imageInfo,
                new byte[0],
                123,
                0,
                "My custom layout",
                "Subtitle My custom layout",
                DateTime.Now,
                DateTime.Now,
                customKeys,
                new LayoutId("C96D8F7A-3219-45D5-A45E-FF6DF805D7F9"),
                null,
                false,
                true
            );

            _keystrokeInterpreter = new KeystrokeInterpreter();
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_WithLayoutNullParameter_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = _keystrokeInterpreter.GetActions(null, new NemeioIndexKeystroke[0]);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var _ = _keystrokeInterpreter.GetActions(_customLayout, null);
            });
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_WithOnlyOneKey_ReturnOneSubaction()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[1]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _zeroKeyIndex
                }
            });

            resultSubActions.Count.Should().Be(1);
            resultSubActions.First().Data.Should().Be(_zeroKey.Actions.First().Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_WithXUnicodeKey_ReturnXSubactions()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[3]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _zeroKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _aKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _zeroKeyIndex
                }
            });

            resultSubActions.Count.Should().Be(resultSubActions.Count);
            resultSubActions.ElementAt(0).Data.Should().Be(_zeroKey.Actions.First().Subactions.First().Data);
            resultSubActions.ElementAt(1).Data.Should().Be(_aKey.Actions.First().Subactions.First().Data);
            resultSubActions.ElementAt(2).Data.Should().Be(_zeroKey.Actions.First().Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_WithShiftOnKey_ReturnOneSubaction()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[2]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _shiftKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _zeroKeyIndex
                }
            });

            var zeroKeyShiftAction = _zeroKey.Actions.Where(x => x.Modifier == KeyboardModifier.Shift).FirstOrDefault();

            resultSubActions.Count.Should().Be(1);
            resultSubActions.First().Data.Should().Be(zeroKeyShiftAction.Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_WithAltGrOnKey_ReturnOneSubaction()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[2]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _altGrKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _zeroKeyIndex
                }
            });

            var zeroKeyAltGrAction = _zeroKey.Actions.Where(x => x.Modifier == KeyboardModifier.AltGr).FirstOrDefault();

            resultSubActions.Count.Should().Be(1);
            resultSubActions.First().Data.Should().Be(zeroKeyAltGrAction.Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_WithShiftAndAltGrOnKey_ReturnOneSubaction()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[3]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _shiftKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _altGrKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _zeroKeyIndex
                }
            });

            var zeroKeyBothAction = _zeroKey.Actions.Where(x => x.Modifier == (KeyboardModifier.Shift | KeyboardModifier.AltGr)).FirstOrDefault();

            resultSubActions.Count.Should().Be(1);
            resultSubActions.First().Data.Should().Be(zeroKeyBothAction.Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_WithShiftAndAltGrOnSameKey_ReturnOneSubaction()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[2]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _shiftAndAltGrIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _zeroKeyIndex
                }
            });

            var zeroKeyBothAction = _zeroKey.Actions.Where(x => x.Modifier == (KeyboardModifier.Shift | KeyboardModifier.AltGr)).FirstOrDefault();

            resultSubActions.Count.Should().Be(1);
            resultSubActions.First().Data.Should().Be(zeroKeyBothAction.Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_WithShiftModifiersSeparatedByKeys_ReturnTwoSubActions()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[3]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _aKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _shiftKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _zeroKeyIndex
                }
            });

            var aKeyNoneAction = _aKey.Actions.Where(x => x.Modifier == KeyboardModifier.None).FirstOrDefault();
            var zeroKeyShiftAction = _zeroKey.Actions.Where(x => x.Modifier == KeyboardModifier.Shift).FirstOrDefault();

            resultSubActions.Count.Should().Be(2);
            resultSubActions.ElementAt(0).Data.Should().Be(aKeyNoneAction.Subactions.First().Data);
            resultSubActions.ElementAt(1).Data.Should().Be(zeroKeyShiftAction.Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_WithAltGrModifiersSeparatedByKeys_ReturnTwoSubActions()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[3]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _aKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _altGrKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _zeroKeyIndex
                }
            });

            var aKeyNoneAction = _aKey.Actions.Where(x => x.Modifier == KeyboardModifier.None).FirstOrDefault();
            var zeroKeyAltGrAction = _zeroKey.Actions.Where(x => x.Modifier == KeyboardModifier.AltGr).FirstOrDefault();

            resultSubActions.Count.Should().Be(2);
            resultSubActions.ElementAt(0).Data.Should().Be(aKeyNoneAction.Subactions.First().Data);
            resultSubActions.ElementAt(1).Data.Should().Be(zeroKeyAltGrAction.Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_ShiftModifierAfterKey_ReturnTwoSubActions()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[2]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _aKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _shiftKeyIndex
                }
            });

            var aKeyNoneAction = _aKey.Actions.Where(x => x.Modifier == KeyboardModifier.None).FirstOrDefault();
            var shiftNoneAction = _shiftKey.Actions.Where(x => x.Modifier == KeyboardModifier.None).FirstOrDefault();

            resultSubActions.Count.Should().Be(2);
            resultSubActions.ElementAt(0).Data.Should().Be(aKeyNoneAction.Subactions.First().Data);
            resultSubActions.ElementAt(1).Data.Should().Be(shiftNoneAction.Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetActions_AltGrModifierAfterKey_ReturnTwoSubActions()
        {
            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, new NemeioIndexKeystroke[2]
            {
                new NemeioIndexKeystroke()
                {
                    Index = _aKeyIndex
                },
                new NemeioIndexKeystroke()
                {
                    Index = _altGrKeyIndex
                }
            });

            var aKeyNoneAction = _aKey.Actions.Where(x => x.Modifier == KeyboardModifier.None).FirstOrDefault();
            var altGrNoneAction = _altGrKey.Actions.Where(x => x.Modifier == KeyboardModifier.None).FirstOrDefault();

            resultSubActions.Count.Should().Be(2);
            resultSubActions.ElementAt(0).Data.Should().Be(aKeyNoneAction.Subactions.First().Data);
            resultSubActions.ElementAt(1).Data.Should().Be(altGrNoneAction.Subactions.First().Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetAction_WhenOnlyOneAction_FnActions_AreRemoved()
        {
            var keystrokes = new NemeioIndexKeystroke[1]
            {
                new NemeioIndexKeystroke() { Index = _functionKeyIndex }
            };

            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, keystrokes);

            resultSubActions.Count.Should().Be(0);
        }

        [Test]
        public void KeystrokeInterpreter_GetAction_WhenCombinaisonFound_FnActions_AreRemoved()
        {
            var keystrokes = new NemeioIndexKeystroke[2]
            {
                new NemeioIndexKeystroke() { Index = _functionKeyIndex },
                new NemeioIndexKeystroke() { Index = _fiveActionsKeyIndex }
            };

            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, keystrokes);

            resultSubActions.Count.Should().Be(1);
        }

        [Test]
        public void KeystrokeInterpreter_GetAction_WhenCombinaisonFound_FnAction_AreSelected()
        {
            var keystrokes = new NemeioIndexKeystroke[2]
            {
                new NemeioIndexKeystroke() { Index = _functionKeyIndex },
                new NemeioIndexKeystroke() { Index = _fiveActionsKeyIndex }
            };

            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, keystrokes);

            var functionAction = _keyWithFiveActions.Actions.First(x => x.Modifier == KeyboardModifier.Function);
            var functionFirstSubAction = functionAction.Subactions.ElementAt(0);

            resultSubActions.Count.Should().Be(1);
            resultSubActions.ElementAt(0).Data.Should().Be(functionFirstSubAction.Data);
        }

        [Test]
        public void KeystrokeInterpreter_GetAction_WhenFunctionCombinaisonFound_EveryOtherKeyBypassed()
        {
            var keystrokes = new NemeioIndexKeystroke[3]
            {
                new NemeioIndexKeystroke() { Index = _functionKeyIndex },
                new NemeioIndexKeystroke() { Index = _fiveActionsKeyIndex },
                new NemeioIndexKeystroke() { Index = _fiveActionsKeyIndex }
            };

            var resultSubActions = _keystrokeInterpreter.GetActions(_customLayout, keystrokes);

            resultSubActions.Count.Should().Be(1);
        }
    }
}
