using System;

namespace Nemeio.Core.JsonModels
{
    public class BatteryInfo
    {
        public int Level { get; set; }

        public BatteryInfo(byte[] data)
        {
            throw new NotImplementedException();
        }

    }
}
