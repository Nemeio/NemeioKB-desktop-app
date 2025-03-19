using System;
using Nemeio.Core.Services.Batteries;

namespace Nemeio.Core.EventArguments
{
    public class BatteryInformationsChangedEventArgs : EventArgs
    {
        public BatteryInformation Informations { get; private set; }

        public BatteryInformationsChangedEventArgs(BatteryInformation informations)
        {
            Informations = informations;
        }
    }
}
