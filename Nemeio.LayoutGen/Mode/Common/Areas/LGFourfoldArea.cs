using System.Collections.Generic;

namespace Nemeio.LayoutGen.Models.Area
{
    internal class LGFourfoldArea : LGArea
    {
        internal override LGPosition CalculatePosition(LGSubkey subkey, LGSize displaybleItemSize)
        {
            var sizeOfCell = subkey.ParentKey.Size;
            var sizeOfCellWithMargin = new LGSize(
                sizeOfCell.Width - DefaultMargin,
                sizeOfCell.Height - DefaultMargin
            );

            var subCellSize = new LGSize(sizeOfCellWithMargin.Width / 2, sizeOfCellWithMargin.Height / 2);
            var calculatedPoint = CreateNewPosition(displaybleItemSize, subCellSize);

            switch (subkey.Position)
            {
                case LGSubKeyDispositionArea.None:
                    calculatedPoint.Y += sizeOfCellWithMargin.Height / 2;
                    break;
                case LGSubKeyDispositionArea.AltGr:
                    calculatedPoint.X += sizeOfCellWithMargin.Height / 2;
                    calculatedPoint.Y += sizeOfCellWithMargin.Height / 2;
                    break;
                case LGSubKeyDispositionArea.ShiftAndAltGr:
                    calculatedPoint.X += sizeOfCellWithMargin.Height / 2;
                    break;
                case LGSubKeyDispositionArea.Shift:
                default:
                    break;
            }

            return ApplyMargin(calculatedPoint, subkey.Position);
        }

        internal override LGSize GetSizeForKey(LGKey key, LGSubKeyDispositionArea area) => new LGSize(key.Size.Width / 2, key.Size.Height / 2) - DefaultMargin;

        private LGPosition CreateNewPosition(LGSize size, LGSize cellSize)
        {
            var topMargin = cellSize.Height / 2 - (size.Height / 2);
            var leftMargin = cellSize.Width / 2 - (size.Width / 2);

            return new LGPosition(leftMargin, topMargin);
        }

        private LGPosition ApplyMargin(LGPosition point, LGSubKeyDispositionArea area)
        {
            switch (area)
            {
                case LGSubKeyDispositionArea.None:
                    point.X += DefaultMargin;
                    point.Y -= DefaultMargin;
                    break;
                case LGSubKeyDispositionArea.AltGr:
                    point.X -= DefaultMargin;
                    point.Y -= DefaultMargin;
                    break;
                case LGSubKeyDispositionArea.ShiftAndAltGr:
                    point.X -= DefaultMargin;
                    point.Y += DefaultMargin;
                    break;
                case LGSubKeyDispositionArea.Shift:
                    point.X += DefaultMargin;
                    point.Y += DefaultMargin;
                    break;
                default:
                    break;
            }

            return point;
        }
    }
}
