using System.Xml.Serialization;

namespace Nemeio.WinAutoInstaller.Models
{
    public class SoftwareItemInfo
    {
        [XmlElement(ElementName = "item")]
        public SoftwareInfo[] Items { get; set; }
    }
}
