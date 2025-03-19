using System.Collections.Generic;
using System.Linq;
using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Export;
using Nemeio.Core.Layouts.Images;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Layout
{
    public class LayoutExportApiOutDto : BaseOutDto
    {
        [JsonProperty(LayoutExportJsonKeys.Title)]
        public string Title { get; set; }

        [JsonProperty(LayoutExportJsonKeys.MajorVersion)]
        public int MajorVersion { get; set; }

        [JsonProperty(LayoutExportJsonKeys.MinorVersion)]
        public int MinorVersion { get; set; }

        [JsonProperty(LayoutExportJsonKeys.Version)]
        public string Version { get; set; }

        [JsonProperty(LayoutExportJsonKeys.AssociatedLayoutId)]
        public string AssociatedLayoutId { get; set; }

        [JsonProperty(LayoutExportJsonKeys.Keys)]
        public IEnumerable<Key> Keys { get; set; }

        [JsonProperty(LayoutExportJsonKeys.Font)]
        public Font Font { get; set; }

        [JsonProperty(LayoutExportJsonKeys.LinkApplicationPaths)]
        public IEnumerable<string> LinkApplicationPaths { get; set; }

        [JsonProperty(LayoutExportJsonKeys.LinkApplicationEnable)]
        public bool LinkApplicationEnable { get; set; }

        [JsonProperty(LayoutExportJsonKeys.IsDarkMode)]
        public bool IsDarkMode { get; set; }

        [JsonProperty(LayoutExportJsonKeys.ImageType)]
        public LayoutImageType ImageType { get; set; }

        [JsonProperty(LayoutExportJsonKeys.Screen)]
        public ScreenType Screen { get; set; } = ScreenType.Eink;

        public static LayoutExportApiOutDto FromModel(LayoutExport layoutExport)
        {
            return new LayoutExportApiOutDto()
            {
                Title = layoutExport.Title,
                MajorVersion = layoutExport.MajorVersion,
                MinorVersion = layoutExport.MinorVersion,
                Version = layoutExport.Version,
                AssociatedLayoutId = layoutExport.AssociatedLayoutId,
                Keys = layoutExport.Keys,
                Font = layoutExport.Font,
                LinkApplicationPaths = layoutExport.LinkApplicationPaths,
                LinkApplicationEnable = layoutExport.LinkApplicationEnable,
                IsDarkMode = layoutExport.IsDarkMode,
                ImageType = layoutExport.ImageType,
                Screen = layoutExport.Screen
            };
        }

        public static LayoutExport FromDto(LayoutExportApiOutDto layoutExportApiOutDto)
        {
            return new LayoutExport()
            {
                Title = layoutExportApiOutDto.Title,
                MajorVersion = layoutExportApiOutDto.MajorVersion,
                MinorVersion = layoutExportApiOutDto.MinorVersion,
                Version = layoutExportApiOutDto.Version,
                AssociatedLayoutId = layoutExportApiOutDto.AssociatedLayoutId,
                Keys = layoutExportApiOutDto.Keys.ToList(),
                Font = layoutExportApiOutDto.Font,
                LinkApplicationPaths = layoutExportApiOutDto.LinkApplicationPaths,
                LinkApplicationEnable = layoutExportApiOutDto.LinkApplicationEnable,
                IsDarkMode = layoutExportApiOutDto.IsDarkMode,
                ImageType = layoutExportApiOutDto.ImageType,
                Screen = layoutExportApiOutDto.Screen
            };
        }
    }
}
