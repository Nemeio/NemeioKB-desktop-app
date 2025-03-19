using SkiaSharp;

namespace Nemeio.LayoutGen.Extensions
{
    public static class SKCanvasExtensions
    {
        public static void DrawFilledRectangle(this SKCanvas canvas, float x, float y, float width, float height, SKColor color)
        {
            var rect = new SKRect(
                x,
                y,
                x + width,
                y + height
            );

            var paint = new SKPaint()
            {
                Color = color,
                Style = SKPaintStyle.Fill
            };

            canvas.DrawRect(rect, paint);
        }
    }
}
