using Nemeio.Core.Services.AppSettings;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out
{
    public class ApplicationSettingsOutDto : BaseOutDto
    {
        [JsonProperty("language")]
        public LanguageOutDto Language { get; set; }

        [JsonProperty("augmentedEnabled")]
        public bool AugmentedImageEnabled { get; set; }

        [JsonProperty("showGrantPrivilegeWindow")]
        public bool ShowGrantPrivilegeWindow { get; set; }

        public static ApplicationSettingsOutDto FromModel(ApplicationSettings settings)
        {
            return new ApplicationSettingsOutDto()
            {
                Language = LanguageOutDto.FromCulture(settings.Language),
                AugmentedImageEnabled = settings.AugmentedImageEnable,
                ShowGrantPrivilegeWindow = settings.ShowGrantPrivilegeWindow,
            };
        }
    }
}
