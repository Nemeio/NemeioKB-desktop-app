namespace Nemeio.Core.Images
{
    public sealed class ImageAdjustment
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public ImageAdjustment(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
