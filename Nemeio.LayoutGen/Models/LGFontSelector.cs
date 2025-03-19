using System;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Models.Fonts;

namespace Nemeio.LayoutGen.Models
{
    public class LGFontSelector : IFontSelector
    {
        private readonly IFontProvider _fontProvider;
        private readonly LGTypefaceProvider _typefaceProvider;

        public LGFontSelector(IFontProvider fontProvider, LGTypefaceProvider typefaceProvider)
        {
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
            _typefaceProvider = typefaceProvider ?? throw new ArgumentNullException(nameof(typefaceProvider));
        }

        public Font FallbackFontIfNeeded(Font font, string character)
        {
            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            if (character == null)
            {
                throw new ArgumentNullException(nameof(character));
            }

            var lowerName = font.Name.ToLower();
            var fontInfo = _fontProvider.Fonts.FirstOrDefault(x => x.Name.ToLower().Equals(lowerName));

            if (fontInfo == null)
            {
                throw new InvalidOperationException($"Unknow font with name <{font.Name}>");
            }

            if (CharacterIsValidWithFont(fontInfo.Name, character))
            {
                return font;
            }

            //  Current selected font appear to not be compliant with the current character.
            //  We try to find a compliant font.

            var knownFonts = _fontProvider.Fonts.OrderBy(x => x.Priority);

            foreach (var currentFont in knownFonts)
            {
                if (CharacterIsValidWithFont(currentFont.Name, character))
                {
                    return new Font(currentFont.Name, font.Size, font.Bold, font.Italic);
                }
            }

            //  No font found
            //  By default with return priority zero font

            var zeroPriorityFont = _fontProvider.Fonts.First(x => x.Priority == 0);

            return new Font(zeroPriorityFont.Name, font.Size, font.Bold, font.Italic);
        }

        private bool CharacterIsValidWithFont(string fontName, string character)
        {
            var typeface = _typefaceProvider.GetTypefaceByName(fontName);
            var glyph = typeface.GetGlyphs(character);
            return glyph.Length != 0;
        }
    }
}
