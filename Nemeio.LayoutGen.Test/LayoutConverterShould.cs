using System.Collections.Generic;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Enums;
using Nemeio.Core.Images;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Converters;
using Nemeio.LayoutGen.Models;
using Nemeio.LayoutGen.Test.Fakes;

namespace Nemeio.LayoutGen.Test
{
    public abstract class LayoutConverterShould
    {
        internal LGLayout CreateLayout(KeyDisposition keyDisposition, KeyboardModifier keyModifier, KeyboardModifier imageModifier, ILayoutConverter layoutConverter)
        {
            var deviceMap = CreateMap();
            var keys = CreateKeys(keyDisposition, keyModifier);

            var layout = layoutConverter.Convert(imageModifier, keys, FontProvider.GetDefaultFont(), true, deviceMap, new ImageAdjustment(0, 0));

            return layout;
        }

        protected NemeioMap CreateMap()
        {
            var mapFactory = new FakeKeyboardNemeioMap();
            var map = mapFactory.CreateEinkMap();
            var deviceMap = new NemeioMap(map);

            return deviceMap;
        }

        protected List<Key> CreateKeys(KeyDisposition disposition, KeyboardModifier modifier)
        {
            var keys = new List<Key>()
            {
                new Key()
                {
                    Index = 31,
                    Font = FontProvider.GetDefaultFont(),
                    Edited = false,
                    Disposition = disposition,
                    Actions = new List<KeyAction>()
                    {
                        new KeyAction()
                        {
                            Display = "A",
                            Modifier = modifier,
                            Subactions = new List<KeySubAction>()
                        }
                    }
                }
            };

            return keys;
        }
    }
}
