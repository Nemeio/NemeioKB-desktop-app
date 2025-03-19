using System;
using Nemeio.Core.Services.Batteries;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Battery
{
    public class BatteryInformationOutDto
    {
        [JsonProperty("level")]
        public ushort Level { get; set; }

        [JsonProperty("remainingCapacity")]
        public ushort RemainingCapacity { get; set; }

        [JsonProperty("timeToFull")]
        public BatteryTimeOutDto TimeToFull { get; set; }

        [JsonProperty("timeToEmpty")]
        public BatteryTimeOutDto TimeToEmpty { get; set; }

        [JsonProperty("cycles")]
        public ushort Cycles { get; set; }

        [JsonProperty("age")]
        public byte Age { get; set; }

        public static BatteryInformationOutDto FromModel(BatteryInformation informations)
        {
            if (informations == null)
            {
                throw new ArgumentNullException(nameof(informations));
            }

            return new BatteryInformationOutDto()
            {
                Level = informations.Level,
                RemainingCapacity = informations.RemainingCapacity,
                TimeToFull = BatteryTimeOutDto.FromModel(informations.TimeToFull),
                TimeToEmpty = BatteryTimeOutDto.FromModel(informations.TimeToEmpty),
                Cycles = informations.Cycles,
                Age = informations.Age
            };
        }
    }
}
