using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nemeio.UpdateInquiry.Dto.Out
{
    public class BinaryListOutDto
    {
        [JsonProperty("softwares")]
        public IList<SoftwareOutDto> Softwares { get; set; }
    }
}
