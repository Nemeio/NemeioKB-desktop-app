using System;
using Nemeio.Core.Services.Batteries;
using Newtonsoft.Json;

namespace Nemeio.Api.Dto.Out.Battery
{
    public class BatteryTimeOutDto
    {
        [JsonProperty("standBy")]
        public bool StandBy { get; private set; }

        [JsonProperty("interval")]
        public TimeSpan Interval { get; private set; }

        public static BatteryTimeOutDto FromModel(BatteryTime time)
        {
            if (time == null)
            {
                throw new ArgumentNullException(nameof(time));
            }

            return new BatteryTimeOutDto()
            {
                StandBy = time.StandBy,
                Interval = time.Interval
            };
        }
    }
}
