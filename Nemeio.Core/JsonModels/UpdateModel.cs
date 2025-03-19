namespace Nemeio.Core.JsonModels
{
    public class UpdateModel
    {
        public VersionModel Cpu { get; set; }

        public VersionModel Ble { get; set; }

        public SoftwareModel Osx { get; set; }

        public SoftwareModel Win { get; set; }
    }
}
