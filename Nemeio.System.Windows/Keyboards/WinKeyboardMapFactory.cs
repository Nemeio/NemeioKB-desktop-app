using System;
using System.Linq;
using Nemeio.Core.Keyboard.Map;
using Nemeio.LayoutGen.Mapping;
using Nemeio.LayoutGen.Mapping.Dto;

namespace Nemeio.Platform.Windows
{
    public sealed class WinKeyboardMapFactory : KeyboardMapFactory
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
}
