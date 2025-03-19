namespace Nemeio.Core.Keyboard.Map
{
    public class KeyboardButton
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public Size Size { get; private set; }
        public bool IsModifier { get; private set; }
        public bool IsFirstLine { get; private set; }
        public uint KeyCode { get; private set; }
        public string DisplayValue { get; private set; }
        public string DataValue { get; private set; }
        public KeyboardFunctionButton FunctionButton { get; private set; }

        public KeyboardButton(float x, float y, float width, float height, bool isModifier, bool isFirstline, uint keyCode, string displayValue, string dataValue, KeyboardFunctionButton functionButton)
        {
            X = x;
            Y = y;
            Size = new Size(width, height);
            IsModifier = isModifier;
            IsFirstLine = isFirstline;
            KeyCode = keyCode;
            DisplayValue = displayValue;
            DataValue = dataValue;
            FunctionButton = functionButton;
        }
    }
}
