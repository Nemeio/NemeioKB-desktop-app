namespace Nemeio.Core.Keyboard.Map
{
    public sealed class Size
    {
        public float Width { get; private set; }
        public float Height { get; private set; }

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }
}
