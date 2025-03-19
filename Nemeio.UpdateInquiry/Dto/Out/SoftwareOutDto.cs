using Nemeio.UpdateInquiry.Dto.JsonKeys;
using Nemeio.UpdateInquiry.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nemeio.UpdateInquiry.Dto.Out
{
    public class SoftwareOutDto
    {
        [JsonProperty(SoftwareJsonKeys.PlatformJsonKey)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Platform Platform { get; set; }

        [JsonProperty(SoftwareJsonKeys.VersionJsonKey)]
        public string Version { get; set; }

        [JsonProperty(SoftwareJsonKeys.UrlJsonKey)]
        public string Url { get; set; }

        [JsonProperty(SoftwareJsonKeys.ChecksumJsonKey)]
        public string Checksum { get; set; }

        public static SoftwareOutDto FromBinary(Binary binary)
        {
            if (binary == null)
            {
                return null;
            }

            return new SoftwareOutDto()
            {
                Version = binary.Version.ToString(),
                Url = binary.Url,
                Platform = binary.Platform,
                Checksum = binary.Checksum
            };
        }
    }
}
