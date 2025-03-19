using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Api.Extensions;
using Nemeio.Core.Keyboard.Screens;
using Newtonsoft.Json;
using NLayout = Nemeio.Core.Services.Layouts.ILayout;

namespace Nemeio.Api.Dto.Out.Layout
{
    public class LightLayoutApiOutDto : BaseOutDto
    {
        [JsonProperty(LayoutJsonKeys.Id)]
        public string Id { get; set; }

        [JsonProperty(LayoutJsonKeys.Index)]
        public int Index { get; set; }

        [JsonProperty(LayoutJsonKeys.Title)]
        public string Title { get; set; }

        [JsonProperty(LayoutJsonKeys.CategoryId)]
        public int CategoryId { get; set; }

        [JsonProperty(LayoutJsonKeys.CreationDate)]
        public long DateCreation { get; set; }

        [JsonProperty(LayoutJsonKeys.UpdateDate)]
        public long DateUpdate { get; set; }

        [JsonProperty(LayoutJsonKeys.Enable)]
        public bool Enable { get; set; }

        [JsonProperty(LayoutJsonKeys.IsHid)]
        public bool IsHid { get; set; }

        [JsonProperty(LayoutJsonKeys.IsFactory)]
        public bool IsFactory { get; set; }

        [JsonProperty(LayoutJsonKeys.IsMac)]
        public bool IsMac { get; set; }

        [JsonProperty(LayoutJsonKeys.IsDefault)]
        public bool IsDefault { get; private set; }

        [JsonProperty(LayoutJsonKeys.ScreenType)]
        public ScreenType Screen { get; private set; }

        [JsonProperty(LayoutJsonKeys.AdjustmentXPosition)]
        public float AdjustmentXPosition { get; private set; }

        [JsonProperty(LayoutJsonKeys.AdjustmentYPosition)]
        public float AdjustmentYPosition { get; private set; }

        public static LightLayoutApiOutDto FromModel(NLayout layout) => new LightLayoutApiOutDto()
        {
            CategoryId = layout.CategoryId,
            DateCreation = layout.DateCreated.ToTimestamp(),
            DateUpdate = layout.DateUpdated.ToTimestamp(),
            Enable = layout.Enable,
            Id = layout.LayoutId,
            Index = layout.Index,
            Title = layout.Title,
            IsHid = layout.LayoutInfo.Hid,
            IsFactory = layout.LayoutInfo.Factory,
            IsMac = layout.LayoutInfo.Mac,
            IsDefault = layout.IsDefault,
            Screen = layout.LayoutImageInfo.Screen.Type,
            AdjustmentXPosition = layout.LayoutImageInfo.XPositionAdjustment,
            AdjustmentYPosition = layout.LayoutImageInfo.YPositionAdjustement
        };
    }
}
