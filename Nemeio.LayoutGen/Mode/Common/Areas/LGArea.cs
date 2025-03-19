namespace Nemeio.LayoutGen.Models.Area
{
    internal abstract class LGArea
    {
        protected const int DefaultMargin = 5;

        internal abstract LGSize GetSizeForKey(LGKey key, LGSubKeyDispositionArea area);

        internal abstract LGPosition CalculatePosition(LGSubkey subkey, LGSize displaybleItemSize);
    }
}
