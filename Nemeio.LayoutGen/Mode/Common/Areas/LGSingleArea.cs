namespace Nemeio.LayoutGen.Models.Area
{
    internal class LGSingleArea : LGArea
    {
        internal override LGPosition CalculatePosition(LGSubkey subkey, LGSize displaybleItemSize)
        {
            var sizeOfCell = subkey.ParentKey.Size;

            var topMargin = sizeOfCell.Height / 2 - (displaybleItemSize.Height / 2);
            var leftMargin = sizeOfCell.Width / 2 - (displaybleItemSize.Width / 2);

            return new LGPosition(leftMargin, topMargin);
        }

        internal override LGSize GetSizeForKey(LGKey key, LGSubKeyDispositionArea area) => key.Size - DefaultMargin * 2;
    }
}
