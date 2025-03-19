using Nemeio.Core.Models.LayoutWarning;

namespace Nemeio.Api.Dto.Out.Warnings
{
    public class ApplicationPathWarningOutDto : WarningOutDto
    {
        public string ApplicationPath { get; set; }
        public string ApplicationName { get; set; }

        public static ApplicationPathWarningOutDto FromWarning(ApplicationPathWarning warning)
        {
            return new ApplicationPathWarningOutDto()
            {
                Type = warning.Type.ToString(),
                ApplicationPath = warning.ApplicationPath,
                ApplicationName = warning.ApplicationName
            };
        }
    }
}
