using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nemeio.Api.Dto;
using Nemeio.Api.Dto.In.Layout;
using Nemeio.Api.Exceptions;
using Nemeio.Api.PatchApplier;
using Nemeio.Core;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Device;
using Nemeio.Core.Enums;
using Nemeio.Core.Errors;
using Nemeio.Core.Keyboard.Map;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services;
using Nemeio.LayoutGen.Mapping;
using Nemeio.LayoutGen.Mapping.Dto;
using Nemeio.Models.Fonts;
using NUnit.Framework;

namespace Nemeio.Api.Test.Patchers
{
    public sealed class FakeKeyboardNemeioMap : KeyboardMapFactory
    {
        public override KeyboardMap ConvertDto(KeyboardMapDto mapDto)
        {
            if (mapDto == null)
            {
                throw new ArgumentNullException(nameof(mapDto));
            }

            var buttons = mapDto
                .Buttons
                .Select(button =>
                {
                    KeyboardFunctionButton functionButton = null;
                    var keyCode = Convert.ToUInt32(button.Windows.KeyCode, 16);

                    if (button.Windows.Function != null)
                    {
                        functionButton = new KeyboardFunctionButton(
                            button.Windows.Function.DisplayValue,
                            button.Windows.Function.DataValue
                        );
                    }

                    var convertedButton = new KeyboardButton(
                        button.X, button.Y, button.Width, button.Height,
                        button.Windows.IsModifier, button.Windows.IsFirstLine,
                        keyCode,
                        button.Windows.DisplayValue, button.Windows.DataValue,
                        functionButton
                    );

                    return convertedButton;
                })
                .ToList();

            var map = new KeyboardMap(mapDto.Size.Width, mapDto.Size.Height, buttons, mapDto.FlipHorizontal);

            return map;
        }
    }

    [TestFixture]
    public class LayoutKeyPatchApplierShould
    {
        private const int NotFirstLineKeyIndex = 19;
        private const int FirstLineKeyIndex = 2;

        private Key _key;
        private Key _firstLineKey;
        private LayoutKeyPatchApplier _patchApplier;
        private IFontProvider _fontProvider;
        private IErrorManager _mockErrorManager;
        private ILayoutImageGenerator _mockLayoutGenService;
        private IDocument _mockDocument;
        [SetUp]
        public void SetUp()
        {
            var loggerFactory = new LoggerFactory();

            _mockErrorManager = Mock.Of<IErrorManager>();

            Mock.Get(_mockErrorManager)
                .Setup(x => x.GetFullErrorMessage(It.IsAny<ErrorCode>(), It.IsAny<Exception>()))
                .Returns(string.Empty);

            _fontProvider = new FontProvider(loggerFactory, _mockErrorManager, _mockDocument);

            _mockLayoutGenService = Mock.Of<ILayoutImageGenerator>();

            var keyboardMap = new FakeKeyboardNemeioMap().CreateHolitechMap();

            _patchApplier = new LayoutKeyPatchApplier(loggerFactory, _fontProvider, _mockLayoutGenService, keyboardMap);

            _key = new Key()
            {
                Index = NotFirstLineKeyIndex,
                Font = FontProvider.GetDefaultFont(),
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Display = "A",
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("A", KeyActionType.Unicode)
                        }
                    },
                    new KeyAction()
                    {
                        Display = "B",
                        Modifier = KeyboardModifier.Shift,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("B", KeyActionType.Unicode)
                        }
                    }
                }
            };

            _firstLineKey = new Key()
            {
                Index = FirstLineKeyIndex,
                Font = FontProvider.GetDefaultFont(),
                Actions = new List<KeyAction>()
                {
                    new KeyAction()
                    {
                        Display = "A",
                        Modifier = KeyboardModifier.None,
                        Subactions = new List<KeySubAction>()
                        {
                            new KeySubAction("A", KeyActionType.Unicode)
                        }
                    }
                }
            };
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateDisplayWhenActionExists_WorksOk()
        {
            const string newDisplayValue = "Z";

            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Display = newDisplayValue,
                    Subactions = new List<PutLayoutSubactionInDto>()
                }
            };

            var updatedKey = _patchApplier.Patch(updateDto, _key);

            updatedKey.Actions.Count.Should().Be(1);

            var noneAction = updatedKey.Actions.FirstOrDefault(x => x.Modifier == KeyboardModifier.None);
            noneAction.Display.Should().Be(newDisplayValue);

            updatedKey.Edited.Should().BeTrue();

            ActionExists(updatedKey.Actions, KeyboardModifier.None).Should().BeTrue();
            ActionExists(updatedKey.Actions, KeyboardModifier.AltGr).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift | KeyboardModifier.AltGr).Should().BeFalse();
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateFont_WorksOk()
        {
            var newFontName = NemeioConstants.Noto;
            const FontSize newFontSize = FontSize.Large;
            const bool newFontBold = true;
            const bool newFontItalic = true;

            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Display = "A",
                    Subactions = new List<PutLayoutSubactionInDto>()
                }
            };
            updateDto.Font = new PutFontInDto()
            {
                Name = newFontName,
                Size = newFontSize,
                Bold = newFontBold,
                Italic = newFontItalic
            };

            var updatedKey = _patchApplier.Patch(updateDto, _key);

            updatedKey.Edited.Should().BeTrue();

            ActionExists(updatedKey.Actions, KeyboardModifier.None).Should().BeTrue();
            ActionExists(updatedKey.Actions, KeyboardModifier.AltGr).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift | KeyboardModifier.AltGr).Should().BeFalse();

            updatedKey.Font.Name.Should().Be(newFontName);
            updatedKey.Font.Size.Should().Be(newFontSize);
            updatedKey.Font.Bold.Should().Be(newFontBold);
            updatedKey.Font.Italic.Should().Be(newFontItalic);
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateFont_WhichDoesntExists_ThrowsPatchException()
        {
            const string newFontName = "this_font_doesn't_exists.ttf";
            const FontSize newFontSize = FontSize.Large;
            const bool newFontBold = true;
            const bool newFontItalic = true;

            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Display = "A",
                    Subactions = new List<PutLayoutSubactionInDto>()
                }
            };
            updateDto.Font = new PutFontInDto()
            {
                Name = newFontName,
                Size = newFontSize,
                Bold = newFontBold,
                Italic = newFontItalic
            };

            var exception = Assert.Throws<PatchFailedException>(() =>
            {
                _patchApplier.Patch(updateDto, _key);
            });

            exception.ErrorCode.Should().Be((int)LayoutKeyError.InvalidFont);
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateFont_WhenIsNotSet_WorksOk()
        {
            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Display = "A",
                    Subactions = new List<PutLayoutSubactionInDto>()
                }
            };

            var updatedKey = _patchApplier.Patch(updateDto, _key);

            updatedKey.Edited.Should().BeTrue();

            ActionExists(updatedKey.Actions, KeyboardModifier.None).Should().BeTrue();
            ActionExists(updatedKey.Actions, KeyboardModifier.AltGr).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift | KeyboardModifier.AltGr).Should().BeFalse();

            updatedKey.Font.Name.Should().Be(_key.Font.Name);
            updatedKey.Font.Size.Should().Be(_key.Font.Size);
            updatedKey.Font.Bold.Should().Be(_key.Font.Bold);
            updatedKey.Font.Italic.Should().Be(_key.Font.Italic);
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateFont_WhenIsNull_WorksOk()
        {
            var updateDto = new PutLayoutKeyInDto();
            updateDto.Font = null;
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Display = "A",
                    Subactions = new List<PutLayoutSubactionInDto>()
                }
            };

            var updatedKey = _patchApplier.Patch(updateDto, _key);

            updatedKey.Edited.Should().BeTrue();

            ActionExists(updatedKey.Actions, KeyboardModifier.None).Should().BeTrue();
            ActionExists(updatedKey.Actions, KeyboardModifier.AltGr).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift | KeyboardModifier.AltGr).Should().BeFalse();

            updatedKey.Font.Should().NotBeNull();
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateSubActionWhenActionExists_WorksOk()
        {
            const string subActionOneData = "Ctrl";
            const KeyActionType subActionOneType = KeyActionType.Special;

            const string subActionTwoData = "R";
            const KeyActionType subActionTwoType = KeyActionType.Unicode;

            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Subactions = new List<PutLayoutSubactionInDto>()
                    {
                        new PutLayoutSubactionInDto()
                        {
                            Data = subActionOneData,
                            Type = subActionOneType
                        },
                        new PutLayoutSubactionInDto()
                        {
                            Data = subActionTwoData,
                            Type = subActionTwoType
                        }
                    }
                }
            };

            var updatedKey = _patchApplier.Patch(updateDto, _key);

            updatedKey.Edited.Should().BeTrue();
            updatedKey.Actions.Count.Should().Be(1);

            var noneAction = updatedKey.Actions.FirstOrDefault(x => x.Modifier == KeyboardModifier.None);
            noneAction.Subactions.Count.Should().Be(2);
            noneAction.Subactions[0].Data.Should().Be(subActionOneData);
            noneAction.Subactions[0].Type.Should().BeEquivalentTo(subActionOneType);
            noneAction.Subactions[1].Data.Should().Be(subActionTwoData);
            noneAction.Subactions[1].Type.Should().BeEquivalentTo(subActionTwoType);

            ActionExists(updatedKey.Actions, KeyboardModifier.None).Should().BeTrue();
            ActionExists(updatedKey.Actions, KeyboardModifier.AltGr).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift | KeyboardModifier.AltGr).Should().BeFalse();
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateDisplayWhenActionDoesntExistsAndValueDefined_WorksOk()
        {
            const string newDisplayValue = "Z";

            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.AltGr,
                    Display = newDisplayValue,
                    Subactions = new List<PutLayoutSubactionInDto>()
                }
            };

            var updatedKey = _patchApplier.Patch(updateDto, _key);

            updatedKey.Edited.Should().BeTrue();
            updatedKey.Actions.Count.Should().Be(1);

            var altGrAction = updatedKey.Actions.FirstOrDefault(x => x.Modifier == KeyboardModifier.AltGr);
            altGrAction.Display.Should().Be(newDisplayValue);

            ActionExists(updatedKey.Actions, KeyboardModifier.AltGr).Should().BeTrue();
            ActionExists(updatedKey.Actions, KeyboardModifier.None).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift | KeyboardModifier.AltGr).Should().BeFalse();
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateDisplayWhenActionDoesntExistsAndValueNotDefined_WorksOk()
        {
            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.AltGr,
                    Subactions = new List<PutLayoutSubactionInDto>()
                }
            };

            var updatedKey = _patchApplier.Patch(updateDto, _key);

            updatedKey.Edited.Should().BeTrue();
            updatedKey.Actions.Count.Should().Be(1);

            var altGrAction = updatedKey.Actions.FirstOrDefault(x => x.Modifier == KeyboardModifier.AltGr);
            altGrAction.Display.Should().Be(string.Empty);

            ActionExists(updatedKey.Actions, KeyboardModifier.AltGr).Should().BeTrue();
            ActionExists(updatedKey.Actions, KeyboardModifier.None).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift | KeyboardModifier.AltGr).Should().BeFalse();
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateSubActionWhenActionDoesntExists_WorksOk()
        {
            const string subActionOneData = "Ctrl";
            const KeyActionType subActionOneType = KeyActionType.Special;

            const string subActionTwoData = "R";
            const KeyActionType subActionTwoType = KeyActionType.Unicode;

            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.AltGr,
                    Subactions = new List<PutLayoutSubactionInDto>()
                    {
                        new PutLayoutSubactionInDto()
                        {
                            Data = subActionOneData,
                            Type = subActionOneType
                        },
                        new PutLayoutSubactionInDto()
                        {
                            Data = subActionTwoData,
                            Type = subActionTwoType
                        }
                    }
                }
            };

            var updatedKey = _patchApplier.Patch(updateDto, _key);

            updatedKey.Edited.Should().BeTrue();
            updatedKey.Actions.Count.Should().Be(1);

            var altGrAction = updatedKey.Actions.FirstOrDefault(x => x.Modifier == KeyboardModifier.AltGr);
            altGrAction.Subactions.Count.Should().Be(2);
            altGrAction.Subactions[0].Data.Should().Be(subActionOneData);
            altGrAction.Subactions[0].Type.Should().BeEquivalentTo(subActionOneType);
            altGrAction.Subactions[1].Data.Should().Be(subActionTwoData);
            altGrAction.Subactions[1].Type.Should().BeEquivalentTo(subActionTwoType);

            ActionExists(updatedKey.Actions, KeyboardModifier.AltGr).Should().BeTrue();
            ActionExists(updatedKey.Actions, KeyboardModifier.None).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift).Should().BeFalse();
            ActionExists(updatedKey.Actions, KeyboardModifier.Shift | KeyboardModifier.AltGr).Should().BeFalse();
        }

        [Test]
        [TestCase("http://google.fr")]
        [TestCase("http://www.wikipedia.fr")]
        [TestCase("https://www.google.com")]
        public void LayoutKeyPatchApplier_Patch_UpdateWithValidUrl_WorksOk(string url)
        {
            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Subactions = new List<PutLayoutSubactionInDto>()
                    {
                        new PutLayoutSubactionInDto()
                        {
                            Data = url,
                            Type = KeyActionType.Url
                        }
                    }
                }
            };

            Assert.DoesNotThrow(() =>
            {
                _patchApplier.Patch(updateDto, _key);
            });
        }

        [Test]
        [TestCase("this_is_a_fake/url")]
        [TestCase("ws://api/blacklists/")]
        [TestCase("application.exe")]
        [TestCase("program.app")]
        public void LayoutKeyPatchApplier_Patch_UpdateWithInvalidUrl_ThrowsPatchFailedException(string url)
        {
            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Subactions = new List<PutLayoutSubactionInDto>()
                    {
                        new PutLayoutSubactionInDto()
                        {
                            Data = url,
                            Type = KeyActionType.Url
                        }
                    }
                }
            };

            var exception = Assert.Throws<PatchFailedException>(() =>
            {
                _patchApplier.Patch(updateDto, _key);
            });

            exception.ErrorCode.Should().Be((int)LayoutKeyError.InvalidUrl);
        }

        [Test]
        [TestCase(@"M:\this\is\a\fake\path\application.exe")]
        [TestCase(@"N:\application.exe")]
        [TestCase("ws://api/blacklists/application.exe")]
        public void LayoutKeyPatchApplier_Patch_UpdateWithInvalidPath_ThrowsPatchFailedException(string applicationPath)
        {
            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Subactions = new List<PutLayoutSubactionInDto>()
                    {
                        new PutLayoutSubactionInDto()
                        {
                            Data = applicationPath,
                            Type = KeyActionType.Application
                        }
                    }
                }
            };

            var exception = Assert.Throws<PatchFailedException>(() =>
            {
                _patchApplier.Patch(updateDto, _key);
            });

            exception.ErrorCode.Should().Be((int)LayoutKeyError.InvalidPath);
        }

        [Test]
        [TestCase(@"M:\this\is\a\fake\path\")]
        [TestCase(@"N:\")]
        [TestCase("ws://api/blacklists/")]
        [TestCase("application.py")]
        public void LayoutKeyPatchApplier_Patch_UpdateWithFileWhichNotAnExecutable_ThrowsPatchFailedException(string applicationPath)
        {
            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.None,
                    Subactions = new List<PutLayoutSubactionInDto>()
                    {
                        new PutLayoutSubactionInDto()
                        {
                            Data = applicationPath,
                            Type = KeyActionType.Application
                        }
                    }
                }
            };

            var exception = Assert.Throws<PatchFailedException>(() =>
            {
                _patchApplier.Patch(updateDto, _key);
            });

            exception.ErrorCode.Should().Be((int)LayoutKeyError.SelectedFileNotExecutable);
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateFunction_OnFirstLineKey_WorksOk()
        {
            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.Function,
                    Subactions = new List<PutLayoutSubactionInDto>()
                    {
                        new PutLayoutSubactionInDto()
                        {
                            Data = "A",
                            Type = KeyActionType.Unicode
                        }
                    }
                }
            };

            Assert.DoesNotThrow(() =>
            {
                _patchApplier.Patch(updateDto, _firstLineKey);
            });
        }

        [Test]
        public void LayoutKeyPatchApplier_Patch_UpdateFunction_NotOnFirstLineKey_WorksOk()
        {
            var updateDto = new PutLayoutKeyInDto();
            updateDto.Actions = new List<PutLayoutKeyActionInDto>()
            {
                new PutLayoutKeyActionInDto()
                {
                    Modifier = KeyboardModifier.Function,
                    Subactions = new List<PutLayoutSubactionInDto>()
                    {
                        new PutLayoutSubactionInDto()
                        {
                            Data = "A",
                            Type = KeyActionType.Unicode
                        }
                    }
                }
            };

            Assert.DoesNotThrow(() =>
            {
                _patchApplier.Patch(updateDto, _key);
            });
        }

        private bool ActionExists(IEnumerable<KeyAction> actions, KeyboardModifier KeyboardModifier) => actions.FirstOrDefault(x => x.Modifier == KeyboardModifier) != null;

        class TestableNemeioMap : IDeviceKeyMap
        {
            public IList<int> UpdatableKeys { get; set; }

            public IList<int> IsFirstLineKeys { get; set; }

            public IList<int> ModifierKeys { get; set; }

            public IList<uint> Buttons => new List<uint>()
            {

            };

            public TestableNemeioMap() => Reset();

            public bool CanUpdateKey(int keyId) => UpdatableKeys.Contains(keyId);

            public bool IsFirstLineKey(int keyIndex) => IsFirstLineKeys.Contains(keyIndex);

            public void Reset()
            {
                UpdatableKeys = new List<int>();
                IsFirstLineKeys = new List<int>();
                ModifierKeys = new List<int>();
            }

            public bool IsModifierKey(int keyIndex) => ModifierKeys.Contains(keyIndex);
        }
    }
}
