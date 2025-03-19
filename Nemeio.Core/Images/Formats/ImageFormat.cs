namespace Nemeio.Core.Images
{
    public abstract class ImageFormat
    {
        public int ImageBpp { get; private set; }

        public ImageFormat(int bpp)
        {
            ImageBpp = bpp;
        }
    }
}
