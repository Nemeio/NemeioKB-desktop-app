using System.Xml.Serialization;

namespace Nemeio.WinAutoInstaller.Models
{
    public class SoftwareInfo
    {
        [XmlElement(ElementName = "platform")]
        public string Platform { get; set; }

        [XmlElement(ElementName = "version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }

        [XmlElement(ElementName = "checksum")]
        public string Checksum { get; set; }
    }
}
