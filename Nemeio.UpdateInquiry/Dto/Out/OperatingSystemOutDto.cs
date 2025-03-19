using System.Collections.Generic;
using Nemeio.UpdateInquiry.Dto.JsonKeys;
using Nemeio.UpdateInquiry.Models;
using Newtonsoft.Json;

namespace Nemeio.UpdateInquiry.Dto.Out
{
    public class OperatingSystemOutDto
    {
        [JsonProperty(OperatingSystemJsonKeys.SoftwaresKey)]
        public IList<SoftwareOutDto> Softwares { get; set; }

        public static OperatingSystemOutDto FromBinaries(IList<Binary> binaries)
        {
            if (binaries == null)
            {
                return null;
            }

            var softwares = new List<SoftwareOutDto>();

            foreach (var binary in binaries)
            {
                if (binary != null)
                {
                    softwares.Add(
                        SoftwareOutDto.FromBinary(binary)
                    );
                }
            }

            return new OperatingSystemOutDto()
            {
                Softwares = softwares
            };
        }
    }
}
