using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Platform;
using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Api.Dto.Out.Warnings;
using Nemeio.Api.Extensions;
using Nemeio.Core.Keyboard.Screens;
using Nemeio.Core.Layouts.Images;
using Nemeio.Core.Layouts.Images.AugmentedLayout;
using Nemeio.Core.Models.LayoutWarning;
using Nemeio.Core.Services.Layouts;
using Newtonsoft.Json;
using NLayout = Nemeio.Core.Services.Layouts.ILayout;

namespace Nemeio.Api.Dto.Out.Layout
{
    public class LayoutApiOutDto : BaseOutDto
    {
        [JsonProperty(LayoutJsonKeys.Id)]
        public string Id { get; set; }

        [JsonProperty(LayoutJsonKeys.AssociatedId)]
        public string AssociatedId { get; set; }

        [JsonProperty(LayoutJsonKeys.Index)]
        public int Index { get; set; }

        [JsonProperty(LayoutJsonKeys.Title)]
        public string Title { get; set; }

        [JsonProperty(LayoutJsonKeys.Subtitle)]
        public string Subtitle { get; set; }

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

        [JsonProperty(LayoutJsonKeys.LinkApplicationPath)]
        public IEnumerable<string> LinkAppPath { get; set; }

        [JsonProperty(LayoutJsonKeys.LinkApplicationEnable)]
        public bool LinkAppEnable { get; set; }

        [JsonProperty(LayoutJsonKeys.IsDefault)]
        public bool IsDefault { get; private set; }

        [JsonProperty(LayoutJsonKeys.IsDarkMode)]
        public bool IsDarkMode { get; private set; }

        [JsonProperty(LayoutJsonKeys.Font)]
        public LayoutKeyFontOutDto Font { get; private set; }

        [JsonProperty(LayoutJsonKeys.Warnings)]
        public IEnumerable<WarningOutDto> Warnings { get; private set; }

        [JsonProperty(LayoutJsonKeys.ImageType)]
        public LayoutImageType ImageType { get; private set; }

        [JsonProperty(LayoutJsonKeys.SystemLayoutId)]
        public string SystemLayoutId { get; private set; }

        [JsonProperty(LayoutJsonKeys.AugmentedHidEnable)]
        public bool AugmentedHidEnable { get; private set; }

        [JsonProperty(LayoutJsonKeys.ImageTypeAvailability)]
        public AugmentedImageTypeAvailabilityOutDto AugmentedImageTypeAvailability { get; private set; }

        [JsonProperty(LayoutJsonKeys.ScreenType)]
        public ScreenType Screen { get; private set; }

        [JsonProperty(LayoutJsonKeys.AdjustmentXPosition)]
        public float AdjustmentXPosition { get; private set; }

        [JsonProperty(LayoutJsonKeys.AdjustmentYPosition)]
        public float AdjustmentYPosition { get; private set; }

        [JsonProperty(LayoutJsonKeys.Keys)]
        public List<LayoutKeyOutDto> Keys { get; set; }

        [JsonProperty(LayoutJsonKeys.Order)]
        public int Order { get; set; }
        public static LayoutApiOutDto FromModel(NLayout layout)
        {
            AugmentedImageTypeAvailabilityOutDto availability = null;

            if (layout.LayoutInfo.Hid)
            {
                var osLayoutId = layout.LayoutInfo.OsLayoutId;
                var augmentedLayoutImageProvider = Mvx.Resolve<IAugmentedLayoutImageProvider>();

                availability = new AugmentedImageTypeAvailabilityOutDto()
                {
                    Classic = augmentedLayoutImageProvider.AugmentedLayoutImageExists(osLayoutId, LayoutImageType.Classic),
                    Hide = augmentedLayoutImageProvider.AugmentedLayoutImageExists(osLayoutId, LayoutImageType.Hide),
                    Gray = augmentedLayoutImageProvider.AugmentedLayoutImageExists(osLayoutId, LayoutImageType.Gray),
                    Bold = augmentedLayoutImageProvider.AugmentedLayoutImageExists(osLayoutId, LayoutImageType.Bold)
                };
            }
            string[] splitTitle = layout.Title.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return new LayoutApiOutDto()
            {
                CategoryId = layout.CategoryId,
                DateCreation = layout.DateCreated.ToTimestamp(),
                DateUpdate = layout.DateUpdated.ToTimestamp(),
                Enable = layout.Enable,
                Id = layout.LayoutId,
                AssociatedId = layout.AssociatedLayoutId,
                Index = layout.Index,
                Title = layout.Title,
                Subtitle = layout.Subtitle,
                Keys = layout.Keys.Select(x => LayoutKeyOutDto.FromModel(x)).ToList(),
                IsHid = layout.LayoutInfo.Hid,
                IsFactory = layout.LayoutInfo.Factory,
                IsMac = layout.LayoutInfo.Mac,
                LinkAppPath = layout.LayoutInfo.LinkApplicationPaths,
                LinkAppEnable = layout.LayoutInfo.LinkApplicationEnable,
                IsDefault = layout.IsDefault,
                IsDarkMode = layout.LayoutImageInfo.Color.IsBlack(),
                Font = LayoutKeyFontOutDto.FromModel(layout.LayoutImageInfo.Font),
                Warnings = ConvertWarnings(layout),
                ImageType = layout.LayoutImageInfo.ImageType,
                AugmentedHidEnable = layout.LayoutInfo.AugmentedHidEnable,
                AugmentedImageTypeAvailability = availability,
                SystemLayoutId = layout.LayoutInfo.OsLayoutId,
                Screen = layout.LayoutImageInfo.Screen.Type,
                AdjustmentXPosition = layout.LayoutImageInfo.XPositionAdjustment,
                AdjustmentYPosition = layout.LayoutImageInfo.YPositionAdjustement,
                Order = layout.Order,
            };
        }

        private static IEnumerable<WarningOutDto> ConvertWarnings(NLayout layout)
        {
            foreach (var warning in layout.Warnings)
            {
                switch (warning.Type)
                {
                    case LayoutWarningType.LinkApplicationPath:
                        yield return ApplicationPathWarningOutDto.FromWarning((ApplicationPathWarning)warning);
                        break;
                    case LayoutWarningType.KeyActionApplicationPath:
                        yield return KeyApplicationPathWarningOutDto.FromWarning((KeyApplicationPathWarning)warning);
                        break;
                    default:
                        throw new InvalidOperationException("Unknow warnings type");
                }
            }
        }
    }
}
