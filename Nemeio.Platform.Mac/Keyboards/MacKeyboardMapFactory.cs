using System;
using System.Linq;
using Nemeio.Core.Keyboard.Map;
using Nemeio.LayoutGen.Mapping;
using Nemeio.LayoutGen.Mapping.Dto;

namespace Nemeio.Platform.Mac.Keyboards
{
    public sealed class MacKeyboardMapFactory : KeyboardMapFactory
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
                    var keyCode = Convert.ToUInt32(button.MacOS.KeyCode, 16);

                    if (button.MacOS.Function != null)
                    {
                        functionButton = new KeyboardFunctionButton(
                            button.MacOS.Function.DisplayValue,
                            button.MacOS.Function.DataValue
                        );
                    }

                    var convertedButton = new KeyboardButton(
                        button.X, button.Y, button.Width, button.Height,
                        button.MacOS.IsModifier, button.MacOS.IsFirstLine,
                        keyCode,
                        button.MacOS.DisplayValue, button.MacOS.DataValue,
                        functionButton
                    );

                    return convertedButton;
                })
                .ToList();

            var map = new KeyboardMap(mapDto.Size.Width, mapDto.Size.Height, buttons, mapDto.FlipHorizontal);

            return map;
        }
    }
}
