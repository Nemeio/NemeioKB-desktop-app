using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Models.Fonts;
using Nemeio.Core.Services.Layouts;
using Nemeio.Core.Systems.Keyboards;
using Nemeio.LayoutGen.Models;

namespace Nemeio.Layout.Builder.Builders
{
    public class JsonKeysBuilder
    {
        private const ushort ScanCodeMaxValue = ushort.MaxValue;

        private IDeviceMap _deviceMap;
        private IFontProvider _fontProvider;
        private IList<Key> _keys;
        private ISystemKeyboardBuilder _osKeyboardBuilder;
        private OsLayoutId _osLayoutId;

        public JsonKeysBuilder()
        {
            _keys = new List<Key>();
        }

        public JsonKeysBuilder SetOsLayoutId(OsLayoutId osLayoutId)
        {
            _osLayoutId = osLayoutId;

            return this;
        }

        public JsonKeysBuilder SetDeviceMap(IDeviceMap deviceMap)
        {
            _deviceMap = deviceMap;

            return this;
        }

        public JsonKeysBuilder SetFontProvider(IFontProvider fontProvider)
        {
            _fontProvider = fontProvider;

            return this;
        }

        public JsonKeysBuilder SetOsKeyboardBuilder(ISystemKeyboardBuilder osKeyboardBuilder)
        {
            _osKeyboardBuilder = osKeyboardBuilder;

            return this;
        }

        public IEnumerable<Key> Build()
        {
            var keys = new List<Key>();

            ForEachScanCode(_deviceMap, (scanCode) =>
            {
                var keyBuilder = AddKey(scanCode)
                                    .SetFontProvider(_fontProvider)
                                    .SetOsKeyboardBuilder(_osKeyboardBuilder);

                _osKeyboardBuilder.ForEachModifiers((modifier) => 
                {
                    keyBuilder.AddAction(scanCode, modifier);
                });

                var newKey = keyBuilder
                    .OverrideDispositionIfLetterKey()
                    .Build();

                keys.Add(newKey);
            });

            return keys;
        }

        private JsonKeyBuilder AddKey(uint scanCode)
        {
            var isRequired = _deviceMap.RequiredButtons.FirstOrDefault(x => x.ScanCode == scanCode) != null;
            var hasFunction = _deviceMap.FnButtons.FirstOrDefault(x => x.ScanCode == scanCode) != null;

            //  We don't manage function key in custom
            var keyDisposition = _osKeyboardBuilder.GetKeyDisposition(scanCode, isRequired, hasFunction);

            var newKey = new Key()
            {
                Index = _deviceMap.Buttons.IndexOf(scanCode),
                Actions = new List<KeyAction>(),
                Disposition = keyDisposition
            };

            _keys.Add(newKey);

            return new JsonKeyBuilder(newKey, _deviceMap, _osLayoutId);
        }

        private void ForEachScanCode(IDeviceMap deviceMap, Action<uint> action)
        {
            for (uint i = 0; i <= ScanCodeMaxValue; i++)
            {
                var index = deviceMap.Buttons.IndexOf(i);
                if (index != -1)
                {
                    action(i);
                }
            }
        }
    }
}
