using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Extensions;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Keyboards;
using Nemeio.LayoutGen.Models;

namespace Nemeio.Layout.Builder.Builders
{
    public class JsonKeyBuilder
    {
        private Key _key;
        private IDeviceMap _deviceMap;
        private OsLayoutId _osLayoutId;
        private IFontProvider _fontProvider;
        private ISystemKeyboardBuilder _osKeyboardBuilder;
        private bool _hasRequiredKey = false;
        private bool _hasFunctionKey = false;

        public JsonKeyBuilder(Key key, IDeviceMap deviceMap, OsLayoutId osLayoutId)
        {
            _key = key;
            _deviceMap = deviceMap;
            _osLayoutId = osLayoutId;
        }

        public JsonKeyBuilder SetFontProvider(IFontProvider fontProvider)
        {
            _fontProvider = fontProvider;

            return this;
        }

        public JsonKeyBuilder SetOsKeyboardBuilder(ISystemKeyboardBuilder osKeyboardBuilder)
        {
            _osKeyboardBuilder = osKeyboardBuilder;

            return this;
        }

        public JsonKeyBuilder AddAction(uint scanCode, KeyboardModifier modifier)
        {
            var dataValue = _osKeyboardBuilder.GetKeyValue(scanCode, modifier, _osLayoutId);
            var displayValue = _osKeyboardBuilder.GetKeyValue(scanCode, modifier, _osLayoutId);
            if (displayValue == null)
            {
                displayValue = string.Empty;
            }

            var isRequired = AddRequiredKey(scanCode, ref displayValue, ref dataValue);
            if (isRequired)
            {
                _hasRequiredKey = true;
            }

            var hasFunction = _deviceMap.FnButtons.FirstOrDefault(x => x.ScanCode == scanCode) != null;
            if (hasFunction)
            {
                //  If a key has a function, it is necessary to override its layout to make it "Full".
                _key.Disposition = KeyDisposition.Full;
            }

            //  To avoid multiple useless keys on key Ctrl + Menu, ArrowLeft + Start and ArrowRight + End
            if (isRequired && hasFunction && modifier != KeyboardModifier.None && modifier != KeyboardModifier.Function)
            {
                displayValue = string.Empty;
            }

            if (hasFunction && modifier == KeyboardModifier.Function)
            {
                AddFunctionKey(scanCode);
            }
            else if (modifier != KeyboardModifier.Function || (isRequired && modifier == KeyboardModifier.Function))
            {
                var newAction = new KeyAction()
                {
                    Display = displayValue,
                    Modifier = modifier,
                    Subactions = new List<KeySubAction>()
                    {
                        new KeySubAction(
                            dataValue,
                            isRequired ? KeyActionType.Special : KeyActionType.Unicode
                        )
                    }
                };

                _key.Actions.Add(newAction);
            }

            return this;
        }

        private JsonKeyBuilder AddFunctionKey(uint scanCode)
        {
            if (_key.Actions.Any(x => x.Modifier == KeyboardModifier.Function))
            {
                //  Key already have a function action

                return this;
            }

            var functionDisplayValue = string.Empty;
            var functionDataValue = string.Empty;

            var hasFunction = GetFunctionKey(scanCode, ref functionDisplayValue, ref functionDataValue);
            if (hasFunction)
            {
                _hasFunctionKey = true;

                var functionAction = new KeyAction()
                {
                    Display = functionDisplayValue,
                    Modifier = KeyboardModifier.Function,
                    Subactions = new List<KeySubAction>()
                    {
                        new KeySubAction(functionDataValue, KeyActionType.Special)
                    }
                };

                _key.Actions.Add(functionAction);
            }

            return this;
        }

        public Key Build()
        {
            _key.Font = CreateFont(_hasRequiredKey, _hasFunctionKey);

            return _key;
        }

        private bool AddRequiredKey(uint scanCode, ref string displayValue, ref string dataValue)
        {
            var requiredButton = _deviceMap.RequiredButtons.FirstOrDefault(x => x.ScanCode == scanCode);
            if (requiredButton != null)
            {
                displayValue = requiredButton.Value;
                dataValue = requiredButton.Key;

                if (displayValue.IsFileRef())
                {
                    displayValue = $"{KeyAction.EmbeddedPrefix}{displayValue}";
                }

                return true;
            }

            return false;
        }

        public JsonKeyBuilder OverrideDispositionIfLetterKey()
        {
            if (IsLetterKey())
            {
                //  Letter key detected
                //  We override disposition to single
                _key.Disposition = KeyDisposition.Single;
            }

            return this;
        }

        private bool GetFunctionKey(uint scanCode, ref string displayValue, ref string dataValue)
        {
            var functionKey = _deviceMap.FnButtons.FirstOrDefault(x => x.ScanCode == scanCode);
            if (functionKey != null)
            {
                displayValue = functionKey.Value;
                dataValue = functionKey.Key;

                if (displayValue.IsFileRef())
                {
                    displayValue = $"{KeyAction.EmbeddedPrefix}{displayValue}";
                }

                return true;
            }

            return false;
        }

        private Font CreateFont(bool isRequired, bool hasFunction)
        {
            //  By default we create font only for function keys
            //  Standard key inherit font from layout

            if (!isRequired && !hasFunction)
            {
                return null;
            }

            var fontBuild = new FontBuilder(_fontProvider)
                                    .SetIsFunctionKey(hasFunction)
                                    .SetIsRequiredKey(isRequired)
                                    .AdjustIfSpecialKey()
                                    .Build();

            return new Font(fontBuild.Name, fontBuild.Size, fontBuild.Bold, fontBuild.Italic);
        }

        private bool IsLetterKey()
        {
            var noneKey = _key.Actions.FirstOrDefault(x => x.Modifier == KeyboardModifier.None);
            var shiftKey = _key.Actions.FirstOrDefault(x => x.Modifier == KeyboardModifier.Shift);

            if (noneKey != null && shiftKey != null)
            {
                return noneKey.Display.ToUpper().Equals(shiftKey.Display);
            }

            return false;
        }
    }
}
