using System.Collections.Generic;
using Nemeio.Api.Dto.JsonKeys;
using Nemeio.Core;
using Nemeio.Core.Layouts.Images;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.In.Layout
{
    public class PutLayoutApiInDto
    {
        private IEnumerable<string> _linkApplicationPath;

        /// <summary>
        /// Is optional. Must be unique.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.Title, Required = Required.Default)]
        public Optional<string> Title { get; set; }

        /// <summary>
        /// Is optional. .
        /// </summary>
        [JsonProperty(LayoutJsonKeys.Subtitle, Required = Required.Default)]
        public Optional<string> Subtitle { get; set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.CategoryId, Required = Required.Default)]
        public Optional<int> CategoryId { get; set; }

        /// <summary>
        /// Is optional. ALlow to enable or disable layout. When layout is disabled, it's removed from keyboard.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.Enable, Required = Required.Default)]
        public Optional<bool> Enable { get; set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.Index, Required = Required.Default)]
        public Optional<int> Index { get; set; }

        /// <summary>
        /// All linked application. When an app is linked and focus, layout is set on keyboard.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.LinkApplicationPath, Required = Required.Default)]
        public IEnumerable<string> LinkApplicationPath
        {
            get
            {
                return _linkApplicationPath;
            }
            set
            {
                _linkApplicationPath = CoreHelpers.SanitizePathList(value);
            }
        }

        /// <summary>
        /// Is optional.
        /// Layout can have linked application but can not trigger apply layout on application focus.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.LinkApplicationEnable, Required = Required.Default)]
        public Optional<bool> LinkApplicationEnable { get; set; }

        /// <summary>
        /// Is optional.
        /// UI settings. Determine if background color is black (true) or white (false)
        /// </summary>
        [JsonProperty(LayoutJsonKeys.IsDarkMode, Required = Required.Default)]
        public Optional<bool> IsDarkMode { get; set; }

        /// <summary>
        /// Is optional.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.AssociatedId, Required = Required.Default)]
        public Optional<string> AssociatedId { get; set; }

        /// <summary>
        /// Is optional.
        /// UI settings. Font used to display text on layout's image. You can choose only on available fonts.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.Font, Required = Required.Default)]
        public Optional<PutFontInDto> Font { get; set; }

        /// <summary>
        /// Is optional.
        /// UI settings. Each type have different behavior when display on keyboard.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.ImageType, Required = Required.Default)]
        public Optional<LayoutImageType> ImageType { get; set; }

        /// <summary>
        /// Is optional.
        /// UI settings. By default, desktop application create layout's image. But you can use hommade image with this option.
        /// </summary>
        [JsonProperty(LayoutJsonKeys.AugmentedHidEnable, Required = Required.Default)]
        public Optional<bool> AugmentedHidEnable { get; set; }

        [JsonProperty(LayoutJsonKeys.AdjustmentXPosition, Required = Required.Default)]
        public Optional<float> AdjustmentXPosition { get; set; }

        [JsonProperty(LayoutJsonKeys.AdjustmentYPosition, Required = Required.Default)]
        public Optional<float> AdjustmentYPosition { get; set; }
    }
}
