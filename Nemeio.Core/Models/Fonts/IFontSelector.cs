using Nemeio.Core.DataModels.Configurator;

namespace Nemeio.Core.Models.Fonts
{
    public interface IFontSelector
    {
        Font FallbackFontIfNeeded(Font font, string character);
    }
}
