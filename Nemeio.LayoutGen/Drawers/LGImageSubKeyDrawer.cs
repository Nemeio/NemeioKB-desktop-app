using System;
using Nemeio.LayoutGen.Factory;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Drawers
{
    internal class LGImageSubKeyDrawer : LGDrawer<LGImageSubkey>
    {
        public LGImageSubKeyDrawer(SKCanvas canvas) 
            : base(canvas) { }

        internal override void Draw(LGImageSubkey imageSubKey)
        {
            var parentKey = imageSubKey.ParentKey;
            var keyArea = parentKey.GetArea();

            using (var bitmap = LoadBitmap(imageSubKey))
            {
                var drawPosition = keyArea.CalculatePosition(imageSubKey, new LGSize(bitmap.Width, bitmap.Height));

                using (var paint = new SKPaint())
                {
                    paint.ColorFilter = SKColorFilter.CreateBlendMode(imageSubKey.ImageColor, SKBlendMode.SrcIn);
                    
                    _canvas.DrawBitmap(
                        bitmap,
                        new SKPoint(parentKey.Position.X + drawPosition.X, parentKey.Position.Y + drawPosition.Y),
                        paint
                    );
                }
            }
        }

        private SKBitmap LoadBitmap(LGImageSubkey imageSubKey)
        {
            const float defaultMargin = 0.40F;

            var size = LGSize.Zero;
            var keyParent = imageSubKey.ParentKey;

            if (keyParent is LGSingleKey)
            {
                size = keyParent.Size * defaultMargin;
            }
            else if (keyParent is LGDoubleKey)
            {
                size = new LGSize(keyParent.Size.Width * defaultMargin);
            }
            else if (keyParent is LGFourfoldKey || keyParent is LGThreeKey)
            {
                size = new LGSize(keyParent.Size.Width * defaultMargin, keyParent.Size.Height * defaultMargin);
            }
            else
            {
                throw new InvalidOperationException("Invalid key");
            }

            var loader = new LGImageLoaderFactory().CreateImageLoader(imageSubKey.Image);

            if (imageSubKey.ImageStream == null)
            {
                return loader.LoadImage(imageSubKey.Image, size);
            }
            else
            {
                using (var imageStream = imageSubKey.ImageStream)
                {
                    return loader.LoadImage(imageSubKey.ImageStream, size);
                }
            }
        }
    }
}
