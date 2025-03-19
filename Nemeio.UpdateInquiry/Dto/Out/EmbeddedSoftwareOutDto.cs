using Nemeio.UpdateInquiry.Builders;
using Nemeio.UpdateInquiry.Dto.JsonKeys;
using Nemeio.UpdateInquiry.Models;
using Newtonsoft.Json;

namespace Nemeio.UpdateInquiry.Dto.Out
{
    public class EmbeddedSoftwareOutDto
    {
        [JsonProperty(EmbeddedSoftwareJsonKeys.VersionJsonKey)]
        public string Version { get; set; }

        [JsonProperty(EmbeddedSoftwareJsonKeys.UrlJsonKey)]
        public string Url { get; set; }

        [JsonProperty(EmbeddedSoftwareJsonKeys.ChecksumJsonKey)]
        public string Checksum { get; set; }

        public static EmbeddedSoftwareOutDto FromBinary(Binary binary)
        {
            if (binary == null)
            {
                return null;
            }

            return new EmbeddedSoftwareOutDto()
            {
                Version = binary.Version.ToString(),
                Url = binary.Url,
                Checksum = binary.Checksum
            };
        }
    }
}
