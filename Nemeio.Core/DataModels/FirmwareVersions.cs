using Nemeio.Core.Keyboard.Screens;

namespace Nemeio.Core.DataModels
{
    public class FirmwareVersions
    {
        public VersionProxy Stm32 { get; set; }
        public VersionProxy Nrf { get; set; }
        public VersionProxy Ite { get; set; }
        public ScreenType ScreenType { get; set; }
        public string Waveform { get; set; }

        public override string ToString()
        {
            return $"Stm32: <{Stm32}>, Nrf: <{Nrf}>, Ite: <{Ite}>, Waveform: <{Waveform}>";
        }
    }
}
