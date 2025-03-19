using Nemeio.UpdateInquiry.Dto.JsonKeys;
using Newtonsoft.Json;

namespace Nemeio.UpdateInquiry.Dto.Out
{
    public class UpdateOutDto
    {
        [JsonProperty(UpdateJsonKeys.CpuJsonKey)]
        public EmbeddedSoftwareOutDto Cpu { get; set; }

        [JsonProperty(UpdateJsonKeys.ScratchInstaller)]
        public EmbeddedSoftwareOutDto ScratchInstaller { get; set; }

        [JsonProperty(UpdateJsonKeys.BleJsonKey)]
        public EmbeddedSoftwareOutDto Ble { get; set; }

        [JsonProperty(UpdateJsonKeys.BluetoothLECompleteJsonKey)]
        public EmbeddedSoftwareOutDto BluetoothLEComplete { get; set; }

        [JsonProperty(UpdateJsonKeys.IteJsonKey)]
        public EmbeddedSoftwareOutDto Ite { get; set; }

        [JsonProperty(UpdateJsonKeys.OsxJsonKey)]
        public OperatingSystemOutDto Osx { get; set; }

        [JsonProperty(UpdateJsonKeys.WindowsJsonKey)]
        public OperatingSystemOutDto Windows { get; set; }

        [JsonProperty(UpdateJsonKeys.IsoMiniInstallerJsonKey)]
        public SoftwareOutDto IsoMiniInstaller { get; set; }

        [JsonProperty(UpdateJsonKeys.SfbJsonKey)]
        public SoftwareOutDto Sfb { get; set; }
        [JsonProperty(UpdateJsonKeys.NrfScratchInstaller)]
        public EmbeddedSoftwareOutDto NrfScratchInstaller { get; set; }
        [JsonProperty(UpdateJsonKeys.WindowsCliJsonKey)]
        public SoftwareOutDto CliWindows { get; set; }
        [JsonProperty(UpdateJsonKeys.LinuxCliJsonKey)]
        public SoftwareOutDto CliLinux { get; set; }
        [JsonProperty(UpdateJsonKeys.WindowsImageLoaderJsonKey)]
        public SoftwareOutDto WindowsImageLoader { get; set; }
        [JsonProperty(UpdateJsonKeys.LinuxImageLoaderJSonKey)]
        public SoftwareOutDto LinuxImageLoader { get; set; }
        [JsonProperty(UpdateJsonKeys.WindowsPackageBuilderJsonKey)]
        public SoftwareOutDto WindowsImagePackageBuilder { get; set; }
        [JsonProperty(UpdateJsonKeys.LinuxPackageBuilderJsonKey)]
        public SoftwareOutDto LinuxPackageBuilder { get; set; }
        [JsonProperty(UpdateJsonKeys.PackageJsonKey)]
        public SoftwareOutDto Package { get; set; }
        [JsonProperty(UpdateJsonKeys.PackageZipJsonKey)]
        public SoftwareOutDto PackageZip { get; set; }
    }
}
