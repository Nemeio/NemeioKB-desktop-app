using System.Collections.Generic;

namespace Nemeio.Core.Models.Fonts
{
    public interface IFontProvider
    {
        HashSet<FontInfo> Fonts { get; }

        bool RegisterFont(FontInfo font);

        bool FontExists(string fontName);
        void RefreshFonts();


    }
}
