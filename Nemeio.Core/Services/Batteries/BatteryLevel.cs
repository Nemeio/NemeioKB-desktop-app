using Nemeio.Core.DataModels;

namespace Nemeio.Core.Services.Batteries
{
    public class BatteryLevel : UShortType<BatteryLevel>
    {
        public BatteryLevel(ushort value) 
            : base(value) { }

        public static BatteryLevel NotPlugged => new BatteryLevel(NemeioConstants.NemeioBatteryNotPlugged);
    }
}
