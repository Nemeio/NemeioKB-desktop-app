namespace Nemeio.LayoutGen.Models
{
    public class Button
    {
        public uint ScanCode { get; private set; }
        public string Value { get; private set; }
        public string Key { get; private set; }

        public Button(uint scanCode, string val, string key = null)
        {
            ScanCode = scanCode;
            Value = val;
            Key = key;
        }
    }
}
