using System.Globalization;

namespace Nemeio.Api.Dto.Out
{
    public class LanguageOutDto : BaseOutDto
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }

        public static LanguageOutDto FromCulture(CultureInfo cultureInfo)
        {
            return new LanguageOutDto()
            {
                Code = cultureInfo.Name,
                DisplayName = cultureInfo.NativeName
            };
        }
    }
}
