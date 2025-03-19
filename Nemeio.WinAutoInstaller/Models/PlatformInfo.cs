using System.Xml.Serialization;

namespace Nemeio.WinAutoInstaller.Models
{
    public class PlatformInfo
    {
        [XmlElement(ElementName = "softwares")]
        public SoftwareItemInfo Software { get; set; }
    }
}
