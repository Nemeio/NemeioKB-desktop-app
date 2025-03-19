using System;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Models.Fonts;
using Nemeio.LayoutGen.Models;
using SkiaSharp;

namespace Nemeio.LayoutGen.Drawers
{
    internal class LGTextSubKeyDrawer : LGDrawer<LGTextSubkey>
    {
        private readonly IFontSelector _fontSelector;
        private readonly LGTypefaceProvider _typefaceProvider;
        private readonly IFontProvider _fontProvider;

        public LGTextSubKeyDrawer(SKCanvas canvas, IFontSelector fontSelector, LGTypefaceProvider typefaceProvider, IFontProvider fontProvider) 
            : base(canvas) 
        {
            _fontSelector = fontSelector ?? throw new ArgumentNullException(nameof(fontSelector));
            _typefaceProvider = typefaceProvider ?? throw new ArgumentNullException(nameof(typefaceProvider));
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
        }

        internal override void Draw(LGTextSubkey textSubKey)
        {
            var parentKey = textSubKey.ParentKey;
            var keyArea = parentKey.GetArea();

            var text = textSubKey.Text;
            var textBounds = new SKRect();

            var keyFont = textSubKey.ParentKey.Font;
            var layoutFont = textSubKey.ParentKey.Parent.Font;
            var selectedFont = keyFont == null ? layoutFont : keyFont;

            //  We check that the font supports the character. 
            //  If it doesn't, we automatically fallback to the most appropriate font.
            selectedFont = _fontSelector.FallbackFontIfNeeded(selectedFont, text);

            using (var fontPaint = GetPaintFromFont(textSubKey, selectedFont, text))
            {
                fontPaint.Style = SKPaintStyle.Fill;
                fontPaint.Color = textSubKey.TextColor;

                if (textSubKey.ForceBold)
                {
                    fontPaint.TextSize *= 1.1f;
                    fontPaint.FakeBoldText = true;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    fontPaint.MeasureText(text, ref textBounds);
                }

                var drawPosition = keyArea.CalculatePosition(textSubKey, new LGSize(textBounds.Width, textBounds.Height));
                var xPosition = parentKey.Position.X + drawPosition.X - textBounds.Left;
                var yPosition = parentKey.Position.Y + drawPosition.Y - textBounds.Top;

                _canvas.DrawText(
                    text,
                    new SKPoint(xPosition, yPosition),
                    fontPaint
                );
            }
        }

        private SKPaint GetPaintFromFont(LGSubkey subKey, Font font, string text)
        {
            const int numberOfRetry = 20;

            var key = subKey.ParentKey;

            var fontSize = _fontProvider.Fonts.First(x => x.Name.Equals(font.Name))
                                .Appearances.First(x => x.KeySize == key.Size.Height)
                                .SizeAppearances.First(x => x.Size == font.Size);

            SKPaint lastPaintTested = null;

            for (var i = 0; i <= numberOfRetry; i++)
            {
                var textBounds = new SKRect();
                var newFontSize = fontSize.RealSize - i;
                var keyAreaSize = key.GetArea().GetSizeForKey(key, subKey.Position);

                lastPaintTested?.Dispose();
                lastPaintTested = BuildTextPaint(key, font, newFontSize);
                lastPaintTested.MeasureText(text, ref textBounds);

                if (!(textBounds.Width > keyAreaSize.Width || textBounds.Height > keyAreaSize.Height))
                {
                    return lastPaintTested;
                }
            }

            return lastPaintTested;
        }

        private SKPaint BuildTextPaint(LGKey key, Font font, int fontSize)
        {
            const float italicSkew = -0.3f;

            var selectedFontSize = fontSize - (fontSize * key.Reduction());

            return new SKPaint()
            {
                Typeface = _typefaceProvider.GetTypefaceByName(font.Name),
                TextSize = selectedFontSize,
                FakeBoldText = font.Bold,
                TextSkewX = font.Italic ? italicSkew : 0.0f,
                IsAntialias = false
            };
        }
    }
}
