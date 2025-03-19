using System;
using Nemeio.Core.DataModels;

namespace Nemeio.Cli.Keyboards.Commands.Version
{
    internal sealed class VersionOutput
    {
        public string Stm { get; private set; }
        public string Nrf { get; private set; }
        public string Ite { get; private set; }
        public string Waveform { get; private set; }

        public VersionOutput(FirmwareVersions versions)
            : this(versions?.Stm32?.ToString() ?? null, versions?.Nrf?.ToString() ?? null, versions?.Ite?.ToString() ?? null, versions?.Waveform ?? null) { }

        public VersionOutput(string stm, string nrf, string ite, string waveform)
        {
            Stm = stm ?? throw new ArgumentNullException(nameof(stm));
            Nrf = nrf ?? throw new ArgumentNullException(nameof(nrf));
            Ite = ite ?? throw new ArgumentNullException(nameof(ite));
            Waveform = waveform ?? throw new ArgumentNullException(nameof(waveform));
        }
    }
}
