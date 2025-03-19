using System.Xml.Serialization;

namespace Nemeio.LayoutGen.Mapping.Dto
{
    public class ScreenSizeDto
    {
        [XmlAttribute("width")]
        public float Width { get; set; }

        [XmlAttribute("height")]
        public float Height { get; set; }
    }
}
