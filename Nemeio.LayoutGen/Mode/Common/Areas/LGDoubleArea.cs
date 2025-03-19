namespace Nemeio.LayoutGen.Models.Area
{
    internal class LGDoubleArea : LGArea
    {
        internal override LGPosition CalculatePosition(LGSubkey subkey, LGSize displaybleItemSize)
        {
            var x = 0f;
            var y = 0f;

            var parent = subkey.ParentKey;

            if (subkey.Position != LGSubKeyDispositionArea.Shift)
            {
                y = parent.Size.Height / 2;
            }

            var sizeOfCell = new LGSize(parent.Size.Width, parent.Size.Height / 2);

            var topMargin = (sizeOfCell.Height - displaybleItemSize.Height) / 2;
            var leftMargin = (sizeOfCell.Width - displaybleItemSize.Width) / 2;

            x = leftMargin;
            y += topMargin;

            return ApplyMargin(new LGPosition(x, y), subkey.Position);
        }

        internal override LGSize GetSizeForKey(LGKey key, LGSubKeyDispositionArea area) => new LGSize(key.Size.Width, key.Size.Height / 2) - DefaultMargin;

        private LGPosition ApplyMargin(LGPosition point, LGSubKeyDispositionArea dispostionArea)
        {
            switch (dispostionArea)
            {
                case LGSubKeyDispositionArea.None:
                    point.Y -= DefaultMargin;
                    break;
                case LGSubKeyDispositionArea.Shift:
                    point.Y += DefaultMargin;
                    break;
            }

            return point;
        }
    }
}
