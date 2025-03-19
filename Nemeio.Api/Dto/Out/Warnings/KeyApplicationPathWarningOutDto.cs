using Nemeio.Core.Enums;
using Nemeio.Core.Models.LayoutWarning;

namespace Nemeio.Api.Dto.Out.Warnings
{
    public class KeyApplicationPathWarningOutDto : WarningOutDto
    {
        public string ApplicationPath { get; set; }
        public string ApplicationName { get; set; }
        public int KeyIndex { get; set; }
        public KeyboardModifier KeyModifier { get; set; }

        public static KeyApplicationPathWarningOutDto FromWarning(KeyApplicationPathWarning warning)
        {
            return new KeyApplicationPathWarningOutDto()
            {
                Type = warning.Type.ToString(),
                ApplicationPath = warning.ApplicationPath,
                ApplicationName = warning.ApplicationName,
                KeyIndex = warning.KeyIndex,
                KeyModifier = warning.KeyModifier
            };
        }
    }
}
