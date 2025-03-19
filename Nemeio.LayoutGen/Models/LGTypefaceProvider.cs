using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.Models.Fonts;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    public class LGTypefaceProvider
    {
        private readonly IFontProvider _fontProvider;

        public IDictionary<string, SKTypeface> Typefaces { get; private set; }

        public LGTypefaceProvider(IFontProvider fontProvider)
        {
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));

            Typefaces = new Dictionary<string, SKTypeface>();
            LoadTypefaces();
        }

        ~LGTypefaceProvider()
        {
            if (Typefaces != null)
            {
                Typefaces.Clear();
                Typefaces = null;
            }
        }

        public SKTypeface GetTypefaceByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException(nameof(name));
            }

            return Typefaces.First(x => x.Key.Equals(name)).Value;
        }

        private void LoadTypefaces()
        {
            foreach (var font in _fontProvider.Fonts)
            {
                Typefaces.Add(
                    font.Name, 
                    SKTypeface.FromStream(font.FontStream)
                );
            }
        }
    }
}
