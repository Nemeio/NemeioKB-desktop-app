using Newtonsoft.Json;

namespace Nemeio.Tools.Core.Files
{
    public sealed class LayoutFileDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; }

        [JsonProperty("factory")]
        public string Factory { get; set; }

        [JsonProperty("imageBpp")]
        public int ImageBpp { get; set; }

        [JsonProperty("disableModifiers")]
        public string DisableModifiers { get; set; }

        [JsonProperty("hid")]
        public string IsHid { get; set; }

        [JsonProperty("associatedId")]
        public string AssociatedId { get; set; }

        [JsonProperty("backgroundColor")]
        public int BackgroundColor { get; set; }
    }
}
