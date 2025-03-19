namespace Nemeio.LayoutGen.Models
{
    public class LGSize

    {
        public float Width { get; }
        public float Height { get; }

        public LGSize(float squareWidth)
        {
            Width = squareWidth;
            Height = squareWidth;
        }

        public LGSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public static LGSize operator /(LGSize size, float nb)
        {
            return new LGSize(size.Width / nb, size.Height / nb);
        }

        public static LGSize operator *(LGSize size, float nb)
        {
            return new LGSize(size.Width * nb, size.Height * nb);
        }

        public static LGSize operator -(LGSize size, float nb)
        {
            return new LGSize(size.Width - nb, size.Height - nb);
        }

        public static LGSize Zero => new LGSize(0, 0);
    }
}
