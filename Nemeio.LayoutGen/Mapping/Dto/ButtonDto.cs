using System.Xml.Serialization;

namespace Nemeio.LayoutGen.Mapping.Dto
{
    public class ButtonDto
    {
        [XmlAttribute("x")]
        public float X { get; set; }

        [XmlAttribute("y")]
        public float Y { get; set; }

        [XmlAttribute("width")]
        public float Width { get; set; }

        [XmlAttribute("height")]
        public float Height { get; set; }

        [XmlElement("windows")]
        public PlatformDto Windows { get; set; }

        [XmlElement("macOS")]
        public PlatformDto MacOS { get; set; }
    }
}
