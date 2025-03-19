using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Keyboard.Map;

namespace Nemeio.LayoutGen.Models
{
    public class NemeioMap : IDeviceMap
    {
        public LGSize DeviceSize { get; private set; }

        public IList<uint> Buttons => KeyboardButtons
            .Select(kButton => kButton.KeyCode)
            .ToList();

        public IList<Button> RequiredButtons => KeyboardButtons
            .Where(kButton => IsRequired(kButton))
            .Select(kButton => new Button(kButton.KeyCode, kButton.DisplayValue, kButton.DataValue))
            .ToList();

        public IList<Button> FnButtons => KeyboardButtons
            .Where(kButton => kButton.FunctionButton != null)
            .Select(kButton => new Button(kButton.KeyCode, kButton.FunctionButton.DisplayValue, kButton.FunctionButton.DataValue))
            .ToList();

        private IList<KeyboardButton> KeyboardButtons { get; set; }

        public NemeioMap(KeyboardMap map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            DeviceSize = new LGSize(map.ScreenSize.Width, map.ScreenSize.Height);
            KeyboardButtons = map.Buttons;
        }

        public LGPosition PositionOfButton(uint scanCode)
        {
            var position = LGPosition.Zero;
            var indexOfButton = Buttons.IndexOf(scanCode);

            if (indexOfButton != -1 && KeyboardButtons.Count() > indexOfButton)
            {
                var selectedButton = KeyboardButtons.ElementAt(indexOfButton);

                position.X = selectedButton.X;
                position.Y = selectedButton.Y;
            }

            return position;
        }

        public LGSize SizeOfButton(uint scanCode)
        {
            var size = LGSize.Zero;
            var indexOfButton = Buttons.IndexOf(scanCode);

            if (indexOfButton != -1 && KeyboardButtons.Count() > indexOfButton)
            {
                var selectedButton = KeyboardButtons.ElementAt(indexOfButton);

                size = new LGSize(selectedButton.Size.Width, selectedButton.Size.Height);
            }

            return size;
        }

        public bool IsFirstLineKey(int keyIndex) => KeyboardButtons[keyIndex].IsFirstLine;

        public bool IsModifierKey(int keyIndex) => KeyboardButtons[keyIndex].IsModifier;

        private bool IsRequired(KeyboardButton button)
        {
            if (button == null)
            {
                throw new ArgumentNullException(nameof(button));
            }

            return button.DisplayValue != null && button.DataValue != null;
        }
    }
}
