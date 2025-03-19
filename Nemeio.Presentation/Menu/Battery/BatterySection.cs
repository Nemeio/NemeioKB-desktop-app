namespace Nemeio.Presentation.Menu.Battery
{
    public sealed class BatterySection
    {
        public bool Visible { get; private set; }
        public BatteryImageType Image { get; private set; }
        public string Text { get; private set; }

        public BatterySection(bool visible, BatteryImageType imageType, string text)
        {
            Visible = visible;
            Image = imageType;
            Text = text;
        }
    }
}
