namespace Nemeio.LayoutGen.Models
{
    public class LGPosition
    {
        public float X { get; set; }
        public float Y { get; set; }

        public LGPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static LGPosition operator +(LGPosition pt1, LGPosition pt2)
        {
            var newPt = new LGPosition(
                pt1.X + pt2.X,
                pt1.Y + pt2.Y
            );

            return newPt;
        }

        public static LGPosition Zero => new LGPosition(0, 0);

        public override bool Equals(object obj)
        {
            if (obj is LGPosition pt)
            {
                return X == pt.X && Y == pt.Y;
            }

            return false;
        }

        public override int GetHashCode() => (X * Y).GetHashCode();
    }
}
