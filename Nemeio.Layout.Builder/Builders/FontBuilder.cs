using System;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Models.Fonts;
using Nemeio.Models.Fonts;

namespace Nemeio.Layout.Builder.Builders
{
    public class FontBuilder
    {
        protected bool _isRequiredKey;
        protected bool _isFunctionKey;

        protected string _fontName;
        protected bool _fontIsBold;
        protected bool _fontIsItalic;
        protected FontSize _fontSize;

        public FontBuilder(IFontProvider fontProvider) 
        {
            if (fontProvider == null)
            {
                throw new ArgumentNullException(nameof(fontProvider));
            }

            //  By default we always create layout with priority 0 font and medium size
            var selectedFont = fontProvider.Fonts.First(x => x.Priority == 0);

            _fontName = selectedFont.Name;
            _fontSize = FontSize.Medium;
            _fontIsBold = false;
            _fontIsItalic = false;
        }

        public FontBuilder SetIsRequiredKey(bool state)
        {
            _isRequiredKey = state;

            return this;
        }

        public FontBuilder SetIsFunctionKey(bool state)
        {
            _isFunctionKey = state;

            return this;
        }

        public FontBuilder AdjustIfSpecialKey()
        {
            if (_fontName == null)
            {
                throw new ArgumentNullException(nameof(_fontName), "You must set layout options before");
            }

            if (_isRequiredKey && !_isFunctionKey)
            {
                _fontSize = FontSize.Small;
            }

            return this;
        }
        public Font Build() => new Font(_fontName, _fontSize, _fontIsBold, _fontIsItalic);
    }
}
