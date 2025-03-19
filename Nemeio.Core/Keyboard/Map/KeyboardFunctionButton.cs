namespace Nemeio.Core.Keyboard.Map
{
    public class KeyboardFunctionButton
    {
        public string DisplayValue { get; private set; }
        public string DataValue { get; private set; }

        public KeyboardFunctionButton(string displayValue, string dataValue)
        {
            DisplayValue = displayValue;
            DataValue = dataValue;
        }
    }
}
